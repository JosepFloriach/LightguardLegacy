using UnityEngine;
using System.Collections;

public class BreathingSpace : MonoBehaviour {

	public bool regenerateAir = false;

	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag.Equals("Player")){
			GameManager.player.GetComponent<PlayerController>().setCanDrownInSpace(false);
			if(regenerateAir){
				GameManager.player.GetComponent<PlayerController>().resetBreathing();
			}
		}
	}
	void OnTriggerExit(Collider collider){
		if(collider.gameObject.tag.Equals("Player")){
			GameManager.player.GetComponent<PlayerController>().setCanDrownInSpace(true);
		}
	}
}
