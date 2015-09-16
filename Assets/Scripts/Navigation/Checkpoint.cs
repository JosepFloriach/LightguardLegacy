using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public int checkPointIndex;
	public GameObject checkPointsManager;
	public GameObject planetGO;
	public ParticleSystem onActivationParticleSystem;
	public bool healsPlayer = false;
	private Planet planet;

	void Awake(){
		checkPointsManager.GetComponent<CheckpointManager> ().registerCheckpoint (gameObject,checkPointIndex);
	}

	void Start(){
		planet = planetGO.GetComponent<Planet> ();
	}

	void OnTriggerEnter (Collider col)
	{
		if(col.gameObject.tag == "Player"){

			if(!GameManager.playerHealManager.isActuallyHealing() && GameManager.player.GetComponent<Killable>().proportionHP()<1f && !GameManager.getIsActualPlanetCorrupted() && healsPlayer){
				GameManager.playerHealManager.activateMenuHeal();
			}
			if(onActivationParticleSystem!=null && GameManager.persistentData.playerLastCheckpoint != checkPointIndex){
				GameManager.persistentData.save ();
				onActivationParticleSystem.Play();
			}
			GameManager.persistentData.playerLastCheckpoint = checkPointIndex;
		}
	}

	void OnTriggerExit (Collider col)
	{
		if(col.gameObject.tag == "Player"){
			GameManager.playerHealManager.deactivateMenuHeal();
		}
	}


	public Planet Planet {
		get {
			return planet;
		}
	}
}
