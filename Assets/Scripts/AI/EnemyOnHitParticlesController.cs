using UnityEngine;
using System.Collections;

public class EnemyOnHitParticlesController : MonoBehaviour {

	private GameObject[] particlesOnContundentHit;

	private GameObject[] particlesOnSlashingHit;

	void Start(){
		GameObject[] particlesContundentPrefabs = GameManager.enemyPrefabManager.getParticlesOnContundentHit();

		GameObject[] particlesSlashingPrefab = GameManager.enemyPrefabManager.getParticlesOnSlashingHit();

		particlesOnContundentHit = new GameObject[particlesContundentPrefabs.Length];
		for(int i = 0;i<particlesContundentPrefabs.Length;i++){
			particlesOnContundentHit[i] = GameObject.Instantiate(particlesContundentPrefabs[i]) as GameObject;
			particlesOnContundentHit[i].transform.parent = transform;
		}

		particlesOnSlashingHit = new GameObject[particlesSlashingPrefab.Length];
		for(int i = 0;i<particlesSlashingPrefab.Length;i++){
			particlesOnSlashingHit[i] = GameObject.Instantiate(particlesSlashingPrefab[i]) as GameObject;
			particlesOnSlashingHit[i].transform.parent = transform;
		}
	}

	public void contundentHit(Vector3 position,Vector3 forwardDirection){
		foreach (GameObject particles in particlesOnContundentHit) {
			particles.transform.position = position;
			particles.transform.forward = forwardDirection;
			particles.GetComponent<ParticleSystem>().Play();
		}
	}

	public void slashingHit(Vector3 position,Vector3 forwardDirection){
		foreach (GameObject particles in particlesOnSlashingHit) {
			particles.transform.position = position;
			particles.transform.forward = forwardDirection;
			particles.GetComponent<ParticleSystem>().Play();
		}
	}
}
