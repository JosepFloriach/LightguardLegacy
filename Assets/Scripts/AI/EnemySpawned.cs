using UnityEngine;
using System.Collections;
using System;
public class EnemySpawned : MonoBehaviour {

	public int pointsCost;
	public Action<GameObject> actionToCallOnDie;
	public Action<GameObject> actionToCallOnDespawn;
	public int maximumEnemiesInPlanet;
	public EnemyType enemyType;

	private bool isDespawned = false;
	private float timerCheckPlayerDistance = 0f;


	Killable killable;
	// Use this for initialization
	void Start () {
		killable = GetComponent<Killable> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(killable.isDead() && !isDespawned){
			OnEnemyDead();
		}
		timerCheckPlayerDistance += Time.deltaTime;

		if(timerCheckPlayerDistance>Constants.TIME_BETWEEN_CHECK_PLAYER_DISTANCE_FOR_DESPAWN){
			timerCheckPlayerDistance =  0f;
			if(Vector3.Distance(GameManager.player.transform.position,transform.position)> Constants.SPAWNING_MAXIMUM_DISTANCE_OF_PLAYER && !isDespawned){
				//Debug.Log("Despawned");
				OnDespawn();
			}
		}
	}

	public void OnEnemyDead(){
		isDespawned = true;
		actionToCallOnDie (gameObject);
	}

	public void OnDespawn(){
		actionToCallOnDespawn (gameObject);
	}
}
