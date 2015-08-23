using UnityEngine;
using System.Collections;

public class FirstPlanetFallingFromTheBridge : Cutscene {

	public GameObject bridge; 
	public GameObject fallenRocks;
	public GameObject fallenRocksAfter;
	public GameObject hideOutsidePlane;
	public GameObject planetGettingCorrupted;

	public override void Initialize() {
		identifyier = CutsceneIdentifyier.FirstPlanetFallingFromTheBridge;
	}

	public override void ActivateTrigger() {
	}
}
