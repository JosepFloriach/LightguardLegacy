using UnityEngine;
using System.Collections;

public class ComboManager : MonoBehaviour {

	public float timeToLooseCombo = 1f;
	public ComboStep[] throwComboSteps;

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

	public ComboStep getCurrentComboStep(){
		ComboStep best = null;
		foreach (ComboStep c in throwComboSteps) {
			if(c.minStep<=comboNum){
				if(best == null){
					best = c;
				}else if(best.minStep<=c.minStep){
					best = c;
				}
			}
		}
		return best;
	}
}
