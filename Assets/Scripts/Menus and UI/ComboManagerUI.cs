using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ComboManagerUI : MonoBehaviour {

	public float timeFadeOut = 0.6f;
	public int minComboAppear = 3;
	int lastCombo = 0;
	bool fadingOut = false;
	float fadeOutTimer = 0f;


	bool beating = false;
	Vector3 originalScale;
	Vector3 extraScaleV;
	float beatTimer;
	bool beatingUp;
	float beatTime = 0.1f;
	float extraScale = 1f;
	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		extraScaleV = new Vector3 (extraScale, extraScale, extraScale);
	}
	
	// Update is called once per frame
	void Update () {
		int newComboNum = GameManager.comboManager.getComboNum ();
		if (lastCombo != newComboNum) {
			if(newComboNum==0){
				fadingOut = true;
				fadeOutTimer = 0f;
			}else if(newComboNum>=minComboAppear){
				GetComponent<Text>().text = "Combo x"+newComboNum;
				GetComponent<CanvasGroup>().alpha = 1f;
				fadingOut = false;
				beating = true;
				beatingUp = true;
				beatTimer = 0f;
			}
			lastCombo = newComboNum;
		}

		if(fadingOut){
			fadeOutTimer+=Time.deltaTime;
			if(fadeOutTimer>=timeFadeOut){
				fadingOut = false;
				GetComponent<CanvasGroup>().alpha = 0f;
			}else{
				GetComponent<CanvasGroup>().alpha = 1f-fadeOutTimer;
			}
		}

		if(beating){
			beatTimer+=Time.deltaTime;
			float ratio = beatTimer/beatTime;
			if(beatingUp){
				if(beatTimer>=beatTime){
					beatTimer = 0f;
					beatingUp = false;
				}else{
					transform.localScale = originalScale + (extraScaleV * ratio);
				}
			}else{
				if(beatTimer>=beatTime){
					beatTimer = 0f;
					beating = false;
					transform.localScale = originalScale;
				}else{
					transform.localScale = originalScale + (extraScaleV * (1f-ratio));
				}
			}
		}
	}

}