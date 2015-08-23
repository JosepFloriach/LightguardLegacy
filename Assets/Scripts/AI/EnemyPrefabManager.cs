using UnityEngine;
using System.Collections;

public class EnemyPrefabManager : MonoBehaviour {

	public GameObject[] enemyPrefabs;
	public GameObject spawnBall;

	public GameObject[] particlesOnContundentHit;
	public GameObject[] particlesOnSlashingHit;
	
	void Awake(){
		GameManager.registerEnemyPrefabManager (this);
	}

	public GameObject getPrefab(EnemyType type){
		foreach(GameObject enemy in enemyPrefabs){
			EnemySpawned enemySpawned = enemy.GetComponent<EnemySpawned>();
			if(enemySpawned.enemyType.Equals(type)){
				return enemy;
			}
		}
		return null;
	}

	public GameObject getSpawnBall(){
		return spawnBall;
	}

	public GameObject[] getParticlesOnContundentHit(){
		return particlesOnContundentHit;
	}

	public GameObject[] getParticlesOnSlashingHit(){
		return particlesOnSlashingHit;
	}
}
