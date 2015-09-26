using UnityEngine;
using System.Collections;

public class PlayerHealManager : MonoBehaviour {

	public GameObject menuOnCanHeal;

	private bool isHealing = false;
	private bool isShowing = false;

	// Use this for initialization
	void Awake () {
		GameManager.registerPlayerHealManager (this);
	}
	
	public void healCompletly(){
		StartCoroutine (healCompletlyCo ());
	}

	public bool isActuallyHealing(){
		return isHealing;
	}

	public bool isShowingMenu(){
		return isShowing;
	}

	public void activateMenuHeal(){
		isShowing = true;
		menuOnCanHeal.SetActive (true);
	}

	public void deactivateMenuHeal(){
		isShowing = false;
		menuOnCanHeal.SetActive (false);
	}

	private IEnumerator healCompletlyCo(){
		float timerToChangePosition = 1f;
		float timer = 0f;
		GameManager.inputController.disableInputController (false);
		GameManager.playerController.setLookingToCameraInCranePosition ();
		isHealing = true;
		deactivateMenuHeal ();
		while (timer<timerToChangePosition) {
			timer+=Time.deltaTime;

			yield return null;
		}

		while (GameManager.player.GetComponent<Killable>().proportionHP()<1f) {
			float timeToWaitForGainLife = 1f;

			yield return new WaitForSeconds(timeToWaitForGainLife);
			GUIManager.getPlayingGUI ().GetComponentInChildren <LifeGUIManager> ().healHitPoints(1);
			GameManager.player.GetComponent<Killable>().GainHealth(1);
		}
		GameManager.playerController.resetLookingPosition ();
		GameManager.inputController.enableInputController ();
		isHealing = false;

	}
}
