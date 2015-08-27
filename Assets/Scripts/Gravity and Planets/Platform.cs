using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {
	public float timeOnUse = 4.0f;
	public float timeToReappear = 5.0f;

	private float timeUsed;
	private float timeDisappeared;
	private bool playerOnPlatform;
	// Use this for initialization
	void Start () {
		timeUsed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (playerOnPlatform) {
			Debug.Log ("Tiempo_usado: " + timeUsed);
			timeUsed += Time.deltaTime;
			if (timeUsed >= timeOnUse) {
				timeUsed = 0;
				timeDisappeared = 0;
				playerOnPlatform = false;
				gameObject.GetComponent<BoxCollider>().enabled = false;
				gameObject.GetComponent<MeshRenderer>().enabled = false;
			}
		} else { 
			Debug.Log ("Tiempo_fuera:" + timeDisappeared);
			timeDisappeared += Time.deltaTime;
			if (timeDisappeared >= timeToReappear) {
				timeDisappeared = 0;
				timeUsed = 0;
				gameObject.GetComponent<BoxCollider>().enabled = true;
				gameObject.GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}

	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player") ) {
			playerOnPlatform = true;
		}
	}

	void OnTriggerExit(Collider collider) {
		if(collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
			playerOnPlatform = false;
		}
	}
}
