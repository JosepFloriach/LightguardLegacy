using UnityEngine;
using System.Collections;

public class HideFrontPlanetFaceOnEnter : MonoBehaviour {

	public bool changeLightsInsidePlanet = true;
	private bool isInsidePlanet = false;
	// Use this for initialization
	void Start () {
		
	}

	void enableAll(bool enabled){
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach(Renderer renderer in renderers){
			renderer.enabled = enabled;
		}
	}
	// Update is called once per frame
	void Update () {
		SphereCollider sphereCollider = (SphereCollider)GetComponent<Collider>();
		float sphereRadius = sphereCollider.transform.lossyScale.x * sphereCollider.radius;

		if(Vector3.Distance(transform.position,GameManager.player.transform.position)<sphereRadius){
			if(GetComponent<Renderer>()!=null){
				GetComponent<Renderer>().enabled = false;
			}
			enableAll(false);

			isInsidePlanet = true;
		}else{
			if(GetComponent<Renderer>()!=null){
				GetComponent<Renderer>().enabled = true;
			}
			enableAll(true);
			isInsidePlanet = false;
		}
	}

	public bool getIsInsidePlanet(){
		return isInsidePlanet;
	}
}
