using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HistoryTextManager : MonoBehaviour {


	
	public GameObject historyText;
	public GameObject activeHistoryText;
	
	
	private bool isActive = false;
	private bool fadingIn = false;
	private bool fadingOut = false;
	private float timer = 0f;
	private string textToPut = "";

	private List<HistoryText> texts = new List<HistoryText> (0);
	

	void Awake(){
		GameManager.registerHistoryTextManager (this);
	}
	
	public void activateHistoryText(string text){
		textToPut = text;
		GameManager.inputController.disableInputController ();
		//Fade In

		historyText.SetActive (true);
		historyText.GetComponentInChildren<Text> ().text = textToPut;
		isActive = true;
		historyText.GetComponent<CanvasGroup>().alpha = 0f;
		timer = 0f;
		fadingIn = true;
		activeHistoryText.SetActive (false);
		//Fade Out
	}
	
	void Update(){
		if (isActive) {
			timer+=Time.deltaTime;
			if(fadingIn){
				float ratio = timer/0.5f;
				if(ratio>=1f){
					historyText.GetComponent<CanvasGroup>().alpha = 1f;
					fadingIn = false;
				}else{
					historyText.GetComponent<CanvasGroup>().alpha = ratio;
				}
			}
			
			if(fadingOut){
				float ratio = 1f-(timer/0.5f);
				if(ratio<=0f){
					historyText.GetComponent<CanvasGroup>().alpha = 0f;
					fadingOut = false;
					deactivate();
				}else{
					historyText.GetComponent<CanvasGroup>().alpha = ratio;
				}
			}
		}
	}

	public void inRangeOfActivation(bool isInRange){
		activeHistoryText.SetActive (isInRange);
	}
	
	public void deactivateHistoryText(){
		if (!fadingIn) {
			fadingOut = true;
			timer = 0f;
		}
	}
	
	private void deactivate(){
		activeHistoryText.SetActive (true);
		historyText.SetActive (false);
		isActive = false;
		GameManager.inputController.enableInputController ();
	}
	
	public bool getIsActive(){
		return isActive;
	}

	public void registerHistoryText(HistoryText t){
		texts.Add (t);
	}

	public bool hasAnyInRangeHistoryText(){
		foreach (HistoryText t in texts) {
			if(t.isActive()){
				return true;
			}
		}
		return false;
	}

	public void activateText(){
		foreach (HistoryText t in texts) {
			if(t.isActive()){
				t.showText();
				break;
			}
		}
	}
}
