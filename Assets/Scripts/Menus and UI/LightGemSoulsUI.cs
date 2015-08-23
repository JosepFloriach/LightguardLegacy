using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LightGemSoulsUI : MonoBehaviour {

	int energyNumber = 0;
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
		int newEnergyNumber = GameManager.lightGemSoulsManager.actualLightPoints;
		if(newEnergyNumber!=energyNumber){
			energyNumber = newEnergyNumber;
			GetComponent<Text> ().text = "x " + energyNumber;
			beating = true;
			beatingUp = true;
			beatTimer = 0f;
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
