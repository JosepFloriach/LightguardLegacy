using UnityEngine;
using System.Collections;

public class RotateAroundItself : MonoBehaviour {

	public Vector3 rotationPerSecond;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (rotationPerSecond * Time.deltaTime);
	}
}
