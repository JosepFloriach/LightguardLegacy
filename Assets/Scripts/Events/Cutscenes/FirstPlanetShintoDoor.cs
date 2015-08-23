using UnityEngine;
using System.Collections;

public class FirstPlanetShintoDoor : Cutscene {

	public GameObject shintoDoor;

	public override void Initialize() {
		identifyier = CutsceneIdentifyier.FirstPlanetShintoDoor;
	}
	
	public override void ActivateTrigger() {
	}
}
