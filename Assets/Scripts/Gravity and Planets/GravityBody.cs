using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof (Rigidbody))]
public class GravityBody : MonoBehaviour {

	public LayerMask objectsToCollide;
	public float drag;
	public float gravityMultiplyier = 1f;
	protected GameObject[] planets;
	protected bool isTouchingPlanet;
	protected float minimumPlanetDistance = float.MaxValue;

	protected List<GameObject> collidingObjects = new List<GameObject>(0);
	protected bool hasToApplyForce = true;
	protected bool hasToChangeFacing = true;



	void Start() {
		planets = GravityAttractorsManager.getGravityAttractors ();
		GetComponent<Rigidbody> ().drag = drag;
		onStart ();
	}

	protected virtual void onStart(){

	}

	public virtual void checkTouchEnter(GameObject obj){
		if (obj.layer.Equals(LayerMask.NameToLayer("Planets")) || 
		    obj.layer.Equals(LayerMask.NameToLayer("Enemy")) || 
		    obj.layer.Equals (LayerMask.NameToLayer ("SlidingPath"))) {
				collidingObjects.Add (obj);
				isTouchingPlanet = true;
		}
	}

	public virtual void checkTouchExit(GameObject obj){
		if (obj.layer.Equals(LayerMask.NameToLayer("Planets")) ||
		    obj.layer.Equals(LayerMask.NameToLayer("Enemy")) ||
		    obj.layer.Equals (LayerMask.NameToLayer ("SlidingPath"))) {
			collidingObjects.Remove(obj);

			if(collidingObjects.Count==0){
				isTouchingPlanet = false;
			}
		}
	}

	public void checkForDestroyedObjects(){
		List<GameObject> newList = new List<GameObject> (0);
		foreach(GameObject go in collidingObjects){
			if(go!=null && go.activeInHierarchy && !Physics.GetIgnoreLayerCollision(go.layer,gameObject.layer)){
				newList.Add(go);
			}
		}
		collidingObjects = newList;
		if (collidingObjects.Count == 0) {
			isTouchingPlanet = false;
		}
	}

	void LateUpdate() {
		attract (hasToApplyForce);
	}

	public virtual void attract(bool applyForce){
		
		//transform.parent = null;
		int closePlanets = 0;
		minimumPlanetDistance = float.MaxValue;
		foreach (GameObject planet in planets) {
			GravityAttractor gravityAttractor = planet.GetComponent<GravityAttractor> ();
			float distance = 0f;
			if(gravityAttractor.Attract (transform,out distance,applyForce,hasToChangeFacing,gravityMultiplyier)){
				closePlanets++;
			}
			if(distance<minimumPlanetDistance){
				minimumPlanetDistance = distance;
			}
		}
	}

	public virtual bool isSpaceGravityBody(){
		return false;
	}

	public bool getIsTouchingPlanetOrCharacters(){
		checkForDestroyedObjects ();
		return isTouchingPlanet;
	}

	public bool getIsTouchingPlanet(){
		foreach(GameObject collidingObject in collidingObjects){
			if(collidingObject.layer.Equals(LayerMask.NameToLayer("Planets")) || collidingObject.layer.Equals (LayerMask.NameToLayer ("SlidingPath"))){
				return true;
			}
		}
		return false;
	}

	public float getMinimumPlanetDistance(){
		return minimumPlanetDistance;
	}

	public void setHasToApplyForce(bool apply){
		hasToApplyForce = apply;
	}

	public bool getHasToApplyForce(){
		return hasToApplyForce;
	}

	public void setHasToChangeFacing(bool change){
		hasToChangeFacing = change;
	}

	public bool getHasToChangeFacing(){
		return hasToChangeFacing;
	}
}
