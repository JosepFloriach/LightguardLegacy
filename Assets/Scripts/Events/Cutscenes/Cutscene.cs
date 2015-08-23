using UnityEngine;
using System.Collections;

public enum CutsceneIdentifyier {
	SanctuaryLightGem,
	FirstPlanetBoarHunting,
	FirstPlanetShintoDoor,
	FirstPlanetFallingFromTheBridge,
	FirstPlanetToTheBridge,
	FirstPlanetToTheBridge2,
	FirstPlanetPathBlockedStones,
	FirstPlanetCorruptionSeeping,
	LastPlanetMundusSecondPhase,
	MundusDies,
    TowerPlanetWhiteHeartEncounter,
    TowerPlanetCorruptionChasing,
	TowerPlanetJumpingToMundus,
    FrostFirePlanetPenguinAttack,
	FrostFirePlanetHydraAppearence};

abstract public class Cutscene: MonoBehaviour  {

	public enum EndMode { ByTime, Inheritance };

	public EndMode endMode; 
	public float endTime;
	public bool isActive = true;
	public LayerMask collisionMask;
	public GameObject planetEventsManagerGO;


	public abstract void ActivateTrigger();
	public abstract void Initialize();
	private bool running = false;
	private PlanetEventsManager planetEventsManager;
	protected CutsceneIdentifyier identifyier;

	void Start(){
		planetEventsManager = planetEventsManagerGO.GetComponent<PlanetEventsManager> ();
		Initialize ();
	}

	void OnTriggerEnter(Collider other) {
		OnTriggerActivated (other);
	}

	public void OnTriggerActivated(Collider other){
		if (isActive && !running){
			if(other.gameObject.layer == LayerMask.NameToLayer("Player")) {
				running = true;
				planetEventsManager.informEventActivated(identifyier);
				StartCutscene ();
				if (endMode == EndMode.ByTime) {
					StartCoroutine ("checkTime");
				}
				ActivateTrigger ();
			}
		}
	}


	void StartCutscene() {
		//Stop all player input. 
		if (endMode == EndMode.ByTime) {
			//GameManager.inputController.disableInputController();
		}
	}

	void EndCutScene(){
		//GameManager.inputController.enableInputController();
	}

	IEnumerator checkTime() {
		yield return new WaitForSeconds(endTime);
		running = false;
		EndCutScene();
	}
	


}
