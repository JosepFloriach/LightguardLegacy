using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GravityAttractor))]
[RequireComponent (typeof (SphereCollider))]

public class Planet : MonoBehaviour {
	public bool centerCameraOnLand = false;

	protected GravityAttractor gravityAttractor;
	protected SphereCollider sphereCollider;
	protected HideFrontPlanetFaceOnEnter hideFrontPlanetFaceOnEnter;

	private bool hasInsidePlanet = false;

	void Start(){
		gravityAttractor = GetComponent<GravityAttractor> ();
		sphereCollider = GetComponent<SphereCollider> ();
		hideFrontPlanetFaceOnEnter = GetComponentInChildren<HideFrontPlanetFaceOnEnter> ();
		if(hideFrontPlanetFaceOnEnter!=null){
			hasInsidePlanet = true;
		}
		initialize ();
	}

	protected virtual void initialize(){

	}

	protected virtual void virtualActivate(){
		
	}

	protected virtual void virtualDeactivate(){
		
	}

	public void activate(){
		virtualActivate ();
	}
	
	public void deactivate(){
		virtualDeactivate ();
	}

	public virtual bool isPlanetCorrupted(){
		return false;
	}

	public bool getHasInsidePlanet(){
		return hasInsidePlanet;
	}

	public GravityAttractor getGravityAttractor() {
		return gravityAttractor;
	}

	public SphereCollider getSphereCollider() {
		return sphereCollider;
	}

	public HideFrontPlanetFaceOnEnter getHideFrontPlanetFaceOnEnter(){
		return hideFrontPlanetFaceOnEnter;
	}
}
