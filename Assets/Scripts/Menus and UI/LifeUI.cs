using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour {
	
	float lifePercentage = 1f;
	bool lerping = false;
	float timer = 0f;
	public float timeToLerp = 0.3f;
	float objectiveLife = 0f;
	float lastRatioCalculated = 1f;
	private float lerpTime;
	
	

	// Use this for initialization
	void Start () {
		GetComponent<Image>().fillAmount = lifePercentage;
	}
	
	// Update is called once per frame
	void Update () {
		float newLife = GameManager.player.GetComponent<Killable> ().proportionHP ();
		if(newLife!=objectiveLife){
			objectiveLife = newLife;
			lifePercentage = lastRatioCalculated;
			lerping = true;
			lerpTime = timeToLerp * Mathf.Abs(objectiveLife-lifePercentage);
			timer = 0f;
		}
		
		if(lerping){
			timer+=Time.deltaTime;
			if(timer>=lerpTime){
				lerping = false;
				lifePercentage = objectiveLife;
				GetComponent<Image>().fillAmount = lifePercentage;
			}else{
				lastRatioCalculated = lifePercentage +((timer/lerpTime) * (objectiveLife - lifePercentage));
				GetComponent<Image>().fillAmount = lastRatioCalculated;
			}
		}
	}
}
