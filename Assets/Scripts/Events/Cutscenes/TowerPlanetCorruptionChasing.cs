using UnityEngine;
using System.Collections;

public class TowerPlanetCorruptionChasing : Cutscene {

	public GameObject corruptionPoint;

	// Use this for initialization
	public override void Initialize() {
		identifyier = CutsceneIdentifyier.TowerPlanetCorruptionChasing;
	}
	
	public override void ActivateTrigger() {
	}
}
