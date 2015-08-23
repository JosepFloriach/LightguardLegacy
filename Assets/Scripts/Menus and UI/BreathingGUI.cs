using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BreathingGUI : MonoBehaviour {

	public GameObject breathingProgressBar;
	
	public void setPercentage(float percentage){
		breathingProgressBar.GetComponent<Image> ().fillAmount = percentage;
	}
}
