using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LightGemEnergyUI : MonoBehaviour {
	
	float energyPercentage = 0f;
	bool lerping = false;
	float timer = 0f;
	float timeToLerp = 0.1f;
	float objectiveEnergy = 0f;
	float lastRatioCalculated = 0f;

	

	// Use this for initialization
	void Start () {
		GetComponent<Image>().fillAmount = energyPercentage;
	}
	
	// Update is called once per frame
	void Update () {
		float newEnergyNumber = GameManager.lightGemEnergyManager.pointsPercentage();
		if(newEnergyNumber!=objectiveEnergy){
			objectiveEnergy = newEnergyNumber;
			energyPercentage = lastRatioCalculated;
			lerping = true;
			timer = 0f;

		}

		if(lerping){
			timer+=(Time.deltaTime/Util.getTimeProportion());
			if(timer>=timeToLerp){
				lastRatioCalculated = objectiveEnergy;
				GetComponent<Image>().fillAmount = lastRatioCalculated;
				lerping = false;
				energyPercentage = objectiveEnergy;
			}else{
				lastRatioCalculated = energyPercentage +((timer/timeToLerp) * (objectiveEnergy - energyPercentage));
				GetComponent<Image>().fillAmount = lastRatioCalculated;
			}
		}
	}
}
