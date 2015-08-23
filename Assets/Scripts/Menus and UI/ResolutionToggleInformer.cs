using UnityEngine;
using System.Collections;
public class ResolutionToggleInformer : MonoBehaviour {

	
	public int width,height;
	public OptionsMenuManager menuManager;
	
	public void informButtonToggled(bool toggled){
		if(toggled){
			menuManager.OnResolutionChanged(width,height);
		}
	}
}
