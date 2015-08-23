using UnityEngine;
using System.Collections;

public class QualityToggleInformer : MonoBehaviour {

	public string optionName;
	public OptionsMenuManager menuManager;

	public void informButtonToggled(bool toggled){
		if(toggled){
			menuManager.OnQualityChanged(optionName);
		}
	}
}
