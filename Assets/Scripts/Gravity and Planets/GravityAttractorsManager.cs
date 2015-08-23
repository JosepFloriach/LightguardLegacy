using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GravityAttractorsManager {

	private static List<GameObject> gravityBodies;

	public static void registerNewAttractor(GameObject gameObject){
		if (gravityBodies == null) {
			gravityBodies = new List<GameObject>();
		}
		gravityBodies.Add (gameObject);
	}

	public static GameObject[] getGravityAttractors(){
		return gravityBodies.ToArray ();
	}
}
