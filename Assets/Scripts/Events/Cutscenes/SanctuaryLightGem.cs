using UnityEngine;
using System.Collections;

public class SanctuaryLightGem : Cutscene {

	public GameObject rocksGO;
	public GameObject rocksGOAfter;
	public GameObject lightGemGO;

	public override void Initialize() {
		identifyier = CutsceneIdentifyier.SanctuaryLightGem;
	}

	public override void ActivateTrigger() {
	}
}
