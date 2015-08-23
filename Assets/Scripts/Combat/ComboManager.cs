using UnityEngine;
using System.Collections;

public class ComboManager : MonoBehaviour {

	public float timeToLooseCombo = 1f;

	int comboNum = 0;
	float timerSinceLastComboed;

	void Start(){
		timerSinceLastComboed = 0f;
		GameManager.registerComboManager (gameObject);
	}

	void Update(){

		timerSinceLastComboed += Time.deltaTime;

		if(timerSinceLastComboed>=timeToLooseCombo){
			comboNum = 0;
		}
	}


	public void addCombo(){
		timerSinceLastComboed = 0f;
		comboNum++;
	}


	public int getComboNum(){
		return comboNum;
	}
}
