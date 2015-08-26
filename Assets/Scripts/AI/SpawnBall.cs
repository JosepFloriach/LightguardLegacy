using UnityEngine;
using System.Collections;

public class SpawnBall : MonoBehaviour {

	public GameObject spawned;
	public ParticleSystem particlesOnSpawn;

	public float timeTillSpawn;

	private float timer = 0f;
	private bool activated = false;
	// Use this for initialization
	void Start () {
		GetComponent<ParticleSystem> ().Stop ();
		//transform.parent = spawned.transform;
	}

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Planets"))){
			GetComponent<ParticleSystem> ().Play ();
			particlesOnSpawn.Stop();
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer>timeTillSpawn && !activated){
			activated = true;
			spawned.SetActive(true);
			spawned.transform.position = transform.position;
			Destroy(GetComponent<ParticleSystem>());
			StartCoroutine(destroyAfterTime());
			GetComponent<Collider>().enabled = false;
			GetComponent<Rigidbody>().isKinematic = true;
			transform.parent = spawned.transform;
			particlesOnSpawn.Play ();
		}
	}

    IEnumerator destroyAfterTime(){
		yield return new WaitForSeconds(GetComponent<ParticleSystem> ().startLifetime);
		Destroy (gameObject);
	}
}
