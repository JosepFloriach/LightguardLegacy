using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class OnAnyKeyPressed : MonoBehaviour {

	public string scene;

	void Start() {

	}


	
	// Update is called once per frame
	void Update () {
		if(Input.anyKey){
			GameManager.actualSceneManager.ChangeScene(scene);

		}
	}
}
