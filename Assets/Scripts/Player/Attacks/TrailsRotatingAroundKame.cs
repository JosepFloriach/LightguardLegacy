using UnityEngine;
using System.Collections;

public class TrailsRotatingAroundKame : MonoBehaviour {

	public GameObject objectToRotateAround;
	public float angle = 10f;
	Vector3 axisToRotateAround;
	public bool randomVector = false;


	void Start () {
		axisToRotateAround = transform.position - objectToRotateAround.transform.position;
		//transform.position = transform.position+(0.2f*new Vector3 (Random.value, Random.value, Random.value).normalized);
	}
	
	// Update is called once per frame
	void Update () {
		if(!randomVector) {
			transform.RotateAround (objectToRotateAround.transform.position, objectToRotateAround.transform.forward, angle * Time.deltaTime);
		}else{
			transform.RotateAround (objectToRotateAround.transform.position, axisToRotateAround, angle * Time.deltaTime);
		}
	}
}
