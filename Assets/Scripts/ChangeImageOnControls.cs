using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeImageOnControls : MonoBehaviour {

	public GameObject[] imagesToActivate;

	void OnEnable(){
		if (!hasXBoxControllerConnected ()) {
			foreach(GameObject ri in imagesToActivate){
				ri.SetActive(true);
			}
			gameObject.SetActive(false);
		}
	}

	bool hasXBoxControllerConnected(){
		string[] inputNames = Input.GetJoystickNames ();

		foreach (string s in inputNames) {
			if(s.Contains("XBOX")){
				return true;
			}
		}

		return false;
	}
}
