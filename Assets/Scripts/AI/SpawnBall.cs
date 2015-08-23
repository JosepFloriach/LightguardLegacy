using UnityEngine;
using System.Collections;

public class SpawnBall : MonoBehaviour {

	public GameObject spawned;

	public float timeTillSpawn;

	private float timer = 0f;
	// Use this for initialization
	void Start () {
		GetComponent<ParticleSystem> ().Stop ();
	}

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.layer.Equals(LayerMask.NameToLayer("Planets"))){
			GetComponent<ParticleSystem> ().Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer>timeTillSpawn){
			spawned.SetActive(true);
			spawned.transform.position = transform.position;
			Destroy(gameObject);
		}
	}
}
