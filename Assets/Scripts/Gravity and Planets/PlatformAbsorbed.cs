using UnityEngine;
using System.Collections;
[RequireComponent(typeof(GravityBody))]
public class PlatformAbsorbed : MonoBehaviour {
	public float speed = 3f;

	// Use this for initialization
	Vector3 originalLocalScale;
	ParticleSystem particles;
	bool fastInitialize = false;

	public void initialize(GameObject parent,bool fastInitialize){
		this.fastInitialize = fastInitialize;
		gameObject.transform.parent = parent.transform;
		originalLocalScale = gameObject.transform.localScale;
		gameObject.transform.localScale = Vector3.zero;
		particles = GetComponent<ParticleSystem> ();
		StartCoroutine (startGrow ());	
	}

	IEnumerator startGrow(){
		float time = 2f;
		if (fastInitialize) {
			time = 0.5f;
		} else {
			particles.Play ();
		}

		float timer = 0f;
		
		while (timer<time) {
			timer+=Time.deltaTime;
			float ratio = timer/time;
			transform.localScale = Vector3.Lerp(Vector3.zero,originalLocalScale,ratio);
			yield return null;
		}
		particles.Stop ();
	}

	void Start () {
		GetComponent<GravityBody> ().setHasToApplyForce (false);
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag.Equals("CenterMundusPlanet") || collision.gameObject.tag.Equals("MundusPlanetFragment")) {
			//Destroy(gameObject);
			StartCoroutine(destroyPlatform());
		}else if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Enemy"))) {
			//Destroy(gameObject);
			StartCoroutine(destroyPlatform());
		}
	}

	IEnumerator destroyPlatform(){
		float time = 0.5f;
		float timer = 0f;
		particles.Play ();
		Vector3 originalScale = gameObject.transform.localScale;
		GetComponent<Collider> ().enabled = false;
		while (timer<time) {
			timer+=Time.deltaTime;
			float ratio = timer/time;
			if(ratio>0.5f && particles.isPlaying){
				particles.Stop ();
			}
			transform.localScale = Vector3.Lerp(originalScale,Vector3.zero,ratio);
			yield return null;
		}

		Destroy(gameObject);
	}

	// Update is called once per frame
	void FixedUpdate () {
		GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		transform.position -= speed * transform.up * Time.fixedDeltaTime;
	}
}
