
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (PlanetCorruption))]
public class PlanetSpawnerManager : MonoBehaviour {

	public Wave[] waves;
	//public GameObject[] spawners;
	public GameObject shintoDoor;
	public bool isActive;
	public float timeBetweenWaves = 2f;

	private List<GameObject> currentEnemies = new List<GameObject> (0);

	//public int pointsUntilSealShintoDoor;
	//public int maxPointsSpawnedAtSameTime;

	//public float timeBetweenSpawns;
	//private int ammountOfActualPointsSpawned = 0;
	//private int accumulatedPoints = 0;

	//public List<EnemyTypeAmmount> enemiesAmmount = new List<EnemyTypeAmmount> (0);

	private float timerSpawn = 0f;
	private PlanetCorruption planetCorruption;
	private bool ongoingCurrentWave = false;
	private int currentWave = 0;
	private bool isFinished = false;

	private int totalPoints;
	private int accumulatedPoints;

	void Start(){
		GameManager.registerPlanetSpawner (gameObject);
		totalPoints = 0;
		foreach(Wave wave in waves){
			foreach(EnemyTypeAmmount enemyAmmount in wave.enemies){
				GameObject enemyPrefab = GameManager.enemyPrefabManager.getPrefab(enemyAmmount.type);
				EnemySpawned enemySpawned= enemyPrefab.GetComponent<EnemySpawned>();
				int costPerUnit = enemySpawned.pointsCost;
				totalPoints+= (costPerUnit * enemyAmmount.ammount);
			}
		}
		planetCorruption = GetComponent<PlanetCorruption> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(!GameManager.isGameEnded && isActive && planetCorruption.getSpawningEnabled() && !ongoingCurrentWave){
			//If we have to spawn
			timerSpawn+=Time.deltaTime;
			if(timerSpawn>timeBetweenWaves){
				if(currentWave>=waves.Length){
					isFinished = true;
					isActive = false;
					GUIManager.deactivateCorruptionBar();
					GetComponent<PlanetCorruption>().cleanCorruption();
				}else{
					//If the time between waves has passed and the last wave was finished we spawn the next wave
					ongoingCurrentWave = true;
					if (currentWave==1){
						if(GetComponent<PlanetCorrupted>().getPlanetEventsManager()!=null){
							GetComponent<PlanetCorrupted>().getPlanetEventsManager().firstWaveFinished();
						}
					}else if (currentWave==2){
						if(GetComponent<PlanetCorrupted>().getPlanetEventsManager()!=null){
							GetComponent<PlanetCorrupted>().getPlanetEventsManager().secondWaveFinished();
						}
					}
					SpawnWave(waves[currentWave]);
					timerSpawn = 0f;
				}
			}

			/*timerSpawn += Time.deltaTime;
			if(ammountOfActualPointsSpawned< maxPointsSpawnedAtSameTime && timerSpawn > timeBetweenSpawns){
				timerSpawn = 0f;
				//Check if any of the spawners is in range for spawning
				List<Spawner> inRangeSpawners = new List<Spawner>(0);

				foreach(GameObject spawn in spawners){
					Spawner s = spawn.GetComponent<Spawner>();
					if(s.isInRange()){
						inRangeSpawners.Add(s);
					}
				}
				//We get a random spawner and we make him spawn a random creature
				if(inRangeSpawners.Count>0){
					Spawner randomSpawner = inRangeSpawners[Random.Range(0,inRangeSpawners.Count)];
					EnemyType type;
					int ammountLeft = pointsUntilSealShintoDoor -(ammountOfActualPointsSpawned + accumulatedPoints);
					ammountOfActualPointsSpawned = ammountOfActualPointsSpawned + randomSpawner.spawnRandomEnemy(onEnemyDead,onEnemyDespawned,out type,enemiesAmmount,ammountLeft);
					if(!type.Equals(EnemyType.None)){
						addEnemyType(type);
					}
				}
			}*/
		}
	}

	private void SpawnWave(Wave wave){
		//Check spawning restrictions
		Vector3 spawnPoint = GameManager.player.transform.position + GameManager.player.transform.up * 3f;
		foreach(EnemyTypeAmmount enemyAmmount in wave.enemies){
			for(int i = 0;i<enemyAmmount.ammount;i++){
				GameObject enemy = GameObject.Instantiate(GameManager.enemyPrefabManager.getPrefab(enemyAmmount.type)) as GameObject;
				SpawnEnemy(enemy,spawnPoint);
				currentEnemies.Add(enemy);
			}
		}
		
		currentWave++;
	}

	private void SpawnEnemy(GameObject toSpawn,Vector3 position){
		EnemySpawned spawned = toSpawn.GetComponent<EnemySpawned>();
		spawned.actionToCallOnDie = onEnemyDead;
		spawned.actionToCallOnDespawn = onEnemyDespawned;
		toSpawn.SetActive(false);
		
		GameObject spawnBall = GameObject.Instantiate(GameManager.enemyPrefabManager.getSpawnBall()) as GameObject;
		
		spawnBall.GetComponent<SpawnBall>().spawned = toSpawn;
		float forceX = Random.value-0.5f;
		float forceY = Random.value-0.5f;
		Vector3 force = new Vector3(forceX,forceY,0f) * 4f;
		spawnBall.transform.position = position + force;
		spawnBall.GetComponent<Rigidbody>().AddForce(force,ForceMode.Impulse);
	}

	/*private void addEnemyType(EnemyType type){
		bool found = false;
		foreach(EnemyTypeAmmount ea in enemiesAmmount){
			if(ea.type.Equals(type)){
				ea.ammount++;
				found = true;
				break;
			}
		}
		if(!found){
			EnemyTypeAmmount ea = new EnemyTypeAmmount();
			ea.type = type;
			ea.ammount = 1;
			enemiesAmmount.Add(ea);
		}
	}*/

	/*private void substractEnemyType(EnemyType type){

		bool found = false;
		foreach(EnemyTypeAmmount ea in enemiesAmmount){
			if(ea.type.Equals(type)){
				ea.ammount--;
				found = true;
				break;
			}
		}
	}*/

	public void onEnemyDead(GameObject enemy){
		if (isActive) {
			EnemySpawned enemySpawned = enemy.GetComponent<EnemySpawned> ();
			accumulatedPoints += enemySpawned.pointsCost;
			currentEnemies.Remove(enemy);
			if(currentEnemies.Count==0){
				ongoingCurrentWave = false;
			}
			GUIManager.setPercentageCorruption ((float)accumulatedPoints / (float)totalPoints);
			/*EnemySpawned enemySpawned = enemy.GetComponent<EnemySpawned> ();
			//accumulatedPoints += enemySpawned.pointsCost;
			ammountOfActualPointsSpawned -= enemySpawned.pointsCost;
	
			substractEnemyType (enemySpawned.enemyType);*/
		}
	}

	/*public void incrementAccumulatedPoints(int ammountPoints){
		/*accumulatedPoints += ammountPoints;
		GUIManager.setPercentageCorruption ((float)accumulatedPoints / (float)pointsUntilSealShintoDoor);
		if(accumulatedPoints>=pointsUntilSealShintoDoor){
			isActive = false;
			GUIManager.deactivateCorruptionBar();
			GetComponent<PlanetCorruption>().cleanCorruption();
			//We clean all the enemies of the planet
			foreach(IAController iaController in GameManager.iaManager.getActualAIs()){
				iaController.die(true);
			}
		}
	}*/

	private IEnumerator SpawnEnemyWithDelay(GameObject enemy,Vector3 position){
		yield return new WaitForSeconds(3f);
		SpawnEnemy (enemy, position);
	}

	public void onEnemyDespawned(GameObject enemy){
		if(isActive){
			enemy.GetComponent<IAController>().interruptAttack();
			Vector3 spawnPoint = GameManager.player.transform.position + GameManager.player.transform.up * 3f;
			enemy.SetActive(false);
			StartCoroutine(SpawnEnemyWithDelay(enemy,spawnPoint));
			//SpawnEnemy(enemy,spawnPoint);
		}else{
			onEnemyDead(enemy);
			Debug.Log(currentEnemies.Count);
			Destroy(enemy);
		}
	}


	/*public void shintoDoorEffect(int level){
		if(level>lastLevelShintoActivated){
			lastLevelShintoActivated = level;
			shinto.activateKanjiLevel (level);
		}
	}*/

	public void activate(){
		if(!isFinished){
			isActive = true;
			GUIManager.activateCorruptionBar ();
			GUIManager.setPercentageCorruption ((float)accumulatedPoints / (float)totalPoints);
		}
	}

	public void deactivate(){
		if(isActive){
			if(!isFinished){
				accumulatedPoints = 0;
				currentWave = 0;
				ongoingCurrentWave = false;
				timerSpawn = 0f;
				GUIManager.deactivateCorruptionBarC();
				currentEnemies = new List<GameObject>(0);
			}
		}
		isActive = false;
	}
}
