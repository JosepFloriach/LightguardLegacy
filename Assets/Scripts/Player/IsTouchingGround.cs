using UnityEngine;
using System.Collections;

public class IsTouchingGround : MonoBehaviour {
	public GameObject mainBody;
	GravityBody gravityBody;
	// Use this for initialization
	void Start () {
		gravityBody = mainBody.GetComponent<GravityBody> ();
	}
	void OnTriggerEnter (Collider col)
	{
		if (!col.isTrigger) {
			gravityBody.checkTouchEnter (col.gameObject);
		}
	}
	
	void OnCollisionEnter (Collision col)
	{
		gravityBody.checkTouchEnter (col.gameObject);
	}
	
	void OnTriggerExit(Collider col)
	{
		if (!col.isTrigger) {
			gravityBody.checkTouchExit (col.gameObject);
		}
	}
	
	void OnCollisionExit(Collision col)
	{
		gravityBody.checkTouchExit (col.gameObject);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
