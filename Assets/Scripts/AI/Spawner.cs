using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Spawner : MonoBehaviour {

	public GameObject[] enemyPrefabsCanSpawn;
	public bool isOutsidePlanet;
	public GameObject particlesSpawnerPrefab;

	private List<GameObject> usedParticleEffects = new List<GameObject>(0);

	public bool isInRange(){
		float distanceBetweenPlayerAndSpawnPoint = Vector3.Distance (transform.position, GameManager.player.transform.position);

		if(distanceBetweenPlayerAndSpawnPoint<Constants.SPAWNING_MAXIMUM_DISTANCE_OF_PLAYER && distanceBetweenPlayerAndSpawnPoint > Constants.SPAWNING_MINIMUM_DISTANCE_OF_PLAYER){
			return true;
		}else{
			return false;
		}

	}

	public int spawnRandomEnemy(Action<GameObject> actionToCallOnEnemyDead,Action<GameObject> actionToCallOnEnemyDespawned,out EnemyType enemyTypeSpawned,List<EnemyTypeAmmount> ammounts,int ammountLeft){
		//Spawns a random enemy and returns the weight it has
		List<GameObject> enemiesCanBeSpawned = new List<GameObject> (0);
		foreach(GameObject enemyPrefab in enemyPrefabsCanSpawn){
			EnemyType type = enemyPrefab.GetComponent<EnemySpawned>().enemyType;
			bool found = false;
			if(enemyPrefab.GetComponent<EnemySpawned>().pointsCost<=(ammountLeft)){
				if(enemyPrefab.GetComponent<EnemySpawned>().maximumEnemiesInPlanet==0){
					enemiesCanBeSpawned.Add(enemyPrefab);
				}else{
					foreach(EnemyTypeAmmount eta in ammounts){
						if(eta.type.Equals(type)){
							if(eta.ammount<enemyPrefab.GetComponent<EnemySpawned>().maximumEnemiesInPlanet){
								enemiesCanBeSpawned.Add(enemyPrefab);
							}
							found = true;
							break;
						}
					}
					if(!found){enemiesCanBeSpawned.Add(enemyPrefab);}
				}
			}
		}
		if(enemiesCanBeSpawned.Count>0){
			GameObject randomEnemyToSpawn = enemiesCanBeSpawned [UnityEngine.Random.Range (0, enemiesCanBeSpawned.Count)];
			GameObject newEnemySpawned = GameObject.Instantiate (randomEnemyToSpawn) as GameObject;
			newEnemySpawned.transform.position = transform.position;

			EnemySpawned enemySpawnedC = newEnemySpawned.GetComponent<EnemySpawned> ();
			enemyTypeSpawned = enemySpawnedC.enemyType;
			enemySpawnedC.actionToCallOnDie = actionToCallOnEnemyDead;
			enemySpawnedC.actionToCallOnDespawn = actionToCallOnEnemyDespawned;
			StartCoroutine(spawnOverTime(newEnemySpawned));
			return enemySpawnedC.pointsCost;
		}else{
			enemyTypeSpawned = EnemyType.None;
			return 0;
		}
	}
	private GameObject getParticleEffect(){
		if(usedParticleEffects.Count>0){
			GameObject effect = usedParticleEffects[0];
			usedParticleEffects.Remove(effect);
			return effect;
		}else{
			GameObject particleEffect = GameObject.Instantiate (particlesSpawnerPrefab);
			return particleEffect;
		}
	}

	private IEnumerator spawnOverTime(GameObject enemySpawned){
		enemySpawned.SetActive (false);
		GameObject particleEffect = getParticleEffect ();
		particleEffect.transform.position = enemySpawned.transform.position;
		particleEffect.transform.forward = enemySpawned.transform.up;
		particleEffect.GetComponent<ParticleSystem> ().Play();
		yield return new WaitForSeconds (1f);
		enemySpawned.SetActive (true);
		//enemySpawned.GetComponent<IAController> ().setNotEnabled();
		//Vector3 originalSize = enemySpawned.transform.localScale;
		//enemySpawned.transform.localScale = originalSize / 10f;
		float timer = 0f;
		float timeItTakesToSpawn = 2f;
		while(timer<timeItTakesToSpawn){
			timer+=Time.deltaTime;
			float ratio = timer/timeItTakesToSpawn;
			//enemySpawned.transform.localScale = originalSize * ratio;
			yield return null;
		}
		particleEffect.GetComponent<ParticleSystem> ().Stop();
		usedParticleEffects.Add (particleEffect);
		//enemySpawned.transform.localScale = originalSize;
		//enemySpawned.GetComponent<IAController> ().activate ();
	}
}
