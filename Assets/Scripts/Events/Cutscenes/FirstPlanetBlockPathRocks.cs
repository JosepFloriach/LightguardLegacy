using UnityEngine;
using System.Collections;

public class FirstPlanetBlockPathRocks : Cutscene {

	public GameObject rocks;
	public GameObject protectCollider;

	public override void Initialize() {
		identifyier = CutsceneIdentifyier.FirstPlanetPathBlockedStones;

	}

	public void informRocksHit(){
		foreach(Rigidbody c in rocks.GetComponentsInChildren<Rigidbody>()){
			c.isKinematic = false;
			StartCoroutine(makeDisappearSlowly(c.gameObject,1f));
		}
		GUIManager.deactivateTutorialText ();

		StartCoroutine(makeDisappearSlowly(protectCollider,0.2f));
	}

	IEnumerator makeDisappearSlowly(GameObject go,float time){
		Vector3 originalScale = go.transform.localScale;
		float timer = 0f;
		while(timer<time){
			timer+=Time.deltaTime;
			float invertRatio = 1f-(timer/time);
			go.transform.localScale = originalScale * invertRatio;
			yield return null;
		}
		go.SetActive (false);
	}

	public override void ActivateTrigger() {
	}
}
