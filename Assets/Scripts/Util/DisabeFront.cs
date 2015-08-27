using UnityEngine;
using System.Collections;

public class DisabeFront : MonoBehaviour {

	public GameObject frontface;
	private MeshRenderer mesh;

	// Use this for initialization
	void Start () {
		mesh = frontface.GetComponent<MeshRenderer>();
	}
	
		void OnTriggerEnter(Collider other) {
		
			if (other.tag == "player") {
			print ("should dissable");
				mesh.enabled = false;
			}
		}
		
		void OnTriggerExit(Collider other) {
			if (other.tag == "player") {
				mesh.enabled = true;
			}
		}

	// Update is called once per frame
	void Update () {
	
	}
}
