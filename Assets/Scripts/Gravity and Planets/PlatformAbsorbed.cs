using UnityEngine;
using System.Collections;
[RequireComponent(typeof(GravityBody))]
public class PlatformAbsorbed : MonoBehaviour {
	public float speed = 3f;

	// Use this for initialization
	void Start () {
		GetComponent<GravityBody> ().setHasToApplyForce (false);
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag.Equals("CenterMundusPlanet") || collision.gameObject.tag.Equals("MundusPlanetFragment")) {
			Destroy(gameObject);
		}else if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy"))) {
			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		transform.position -= speed * transform.up * Time.fixedDeltaTime;
	}
}
