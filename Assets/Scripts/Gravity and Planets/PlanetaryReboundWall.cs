using UnityEngine;
using System.Collections;

public class PlanetaryReboundWall : MonoBehaviour {

	public GameObject planetRotatingOver;

	public bool isRotating = false;
	public float speed = 0f;

	public bool useNormal = false;

	void Start(){
		if(planetRotatingOver!=null){
			transform.up = transform.position - planetRotatingOver.transform.position;
		}
	}

	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag == "Player"){
			Vector3 normal;
			if(useNormal){
				normal = transform.up;
			}else{
				normal =(transform.position - planetRotatingOver.transform.position).normalized;
			}
			GameManager.player.GetComponent<Rigidbody>().velocity = Vector3.Reflect(GameManager.player.GetComponent<Rigidbody>().velocity,normal);
		}
	}

	void FixedUpdate(){
		if(isRotating && planetRotatingOver!=null){
			transform.position = OrbitAroundPoint (transform.position, transform.parent.position, Quaternion.Euler (0, 0, speed * Time.deltaTime));
			transform.up = transform.position - planetRotatingOver.transform.position;
		}
	}

	private Vector3 OrbitAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle) {
		return angle * ( point - pivot) + pivot;
	}


}
