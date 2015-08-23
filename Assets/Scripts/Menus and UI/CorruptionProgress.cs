using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CorruptionProgress : MonoBehaviour {

	public GameObject corruptionBar;
	public GameObject corruptionGroup;

	//Beating
	bool beating = false;
	Vector3 originalScale;
	Vector3 extraScaleV;
	float beatTimer;
	bool beatingUp;
	float beatTime = 0.3f;
	float extraScale = 0.1f;

	//Lerp of the progressBar
	float percentage = 0f;
	bool lerping = false;
	float timer = 0f;
	float timeToLerp = 0.3f;
	float objectivePercentage = 0f;
	float lastRatioCalculated = 0f;



	void Start(){
		originalScale = corruptionGroup.transform.localScale;
		extraScaleV = new Vector3 (extraScale, extraScale, extraScale);
		setPercentage (0f);
	}

	void Update(){

		if(lerping){
			timer+=Time.deltaTime;
			if(timer>=timeToLerp){
				lerping = false;
				percentage = objectivePercentage;
			}else{
				lastRatioCalculated = percentage +((timer/timeToLerp) * (objectivePercentage - percentage));
				corruptionBar.GetComponent<Image>().fillAmount = lastRatioCalculated;
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
					corruptionGroup.transform.localScale = originalScale + (extraScaleV * ratio);
				}
			}else{
				if(beatTimer>=beatTime){
					beatTimer = 0f;
					beating = false;
					corruptionGroup.transform.localScale = originalScale;
				}else{
					corruptionGroup.transform.localScale = originalScale + (extraScaleV * (1f-ratio));
				}
			}
		}
	}

	public Vector3 getPixelPositionCorruptionBar(){
		return corruptionGroup.GetComponent<RectTransform>().position;
	}
	public void setPercentage(float percentage){
		//corruptionBar.GetComponent<Image> ().fillAmount = percentage;
		beating = true;
		beatingUp = true;
		beatTimer = 0f;

		objectivePercentage = percentage;
		this.percentage = lastRatioCalculated;
		lerping = true;
		timer = 0f;

	}
}
