using UnityEngine;
using System.Collections;

public class CutsceneCollider : MonoBehaviour {

	public GameObject cutsceneGO;

	private Cutscene cutscene;
	// Use this for initialization
	void Start () {
		cutscene = cutsceneGO.GetComponent<Cutscene>();
	}
	
	void OnTriggerEnter(Collider other){
		cutscene.OnTriggerActivated (other);
	}
}
