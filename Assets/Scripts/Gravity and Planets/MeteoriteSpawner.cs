using UnityEngine;
using System.Collections;

public class MeteoriteSpawner : MonoBehaviour {

	public GameObject meteoritePrefab;

	public GameObject[] positionsToSpawn;
	public float timeBetweenSpawns;
	public bool isSpawningLineal = true;
	public float speed;


	private int index= 0;
	private float timer = 0f;
	public float timeToLast = 30f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer>timeBetweenSpawns){
			SpawnMeteorite();
			timer = 0f;
		}
	}

	private void SpawnMeteorite(){
		GameObject newMeteorite = GameObject.Instantiate (meteoritePrefab) as GameObject;
		Vector3 position = positionsToSpawn [index].transform.position;
		newMeteorite.transform.position = position;

		Vector3 direction = position - transform.position;
		newMeteorite.GetComponent<Meteorite>().initialize(direction,speed,timeToLast);

		index = (index+1) % positionsToSpawn.Length;

	}
}
