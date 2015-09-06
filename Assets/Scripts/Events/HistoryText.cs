using UnityEngine;
using System.Collections;

public class HistoryText : MonoBehaviour {

	private bool isInside = false;

	[TextArea(5,10)]
	public string historyText;

	private bool isPlayerInRange = false;

	void Start(){
		GameManager.historyTextManager.registerHistoryText (this);
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player") {
			isPlayerInRange = true;
			GameManager.historyTextManager.inRangeOfActivation(true);
		}
	}

	void OnTriggerExit(Collider collider){
		if (collider.gameObject.tag == "Player") {
			isPlayerInRange = false;
			GameManager.historyTextManager.inRangeOfActivation(false);
		}
	}

	public bool isActive(){
		return isPlayerInRange;
	}

	public void showText(){
		GameManager.historyTextManager.activateHistoryText (historyText);
	}
}
