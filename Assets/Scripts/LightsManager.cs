using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LightsManager : MonoBehaviour {

	public Color colorAmbientInsidePlanet;
	private List<Light> directionalLights;
	private Color originalAmbientLight;

	void onStart(){
		GameManager.registerLightsManager (this);
	}

	public void enableDirectionalLights(bool enabled){
		if(directionalLights==null){
			originalAmbientLight = RenderSettings.ambientLight;
			directionalLights = new List<Light>(0);
			Light[] lights = GameObject.FindObjectsOfType<Light>();
			foreach(Light light in lights){
				if(light.type.Equals(LightType.Directional)){
					directionalLights.Add(light);
				}
			}
		}

		foreach(Light light in directionalLights){
			light.enabled = enabled;
		}

		if(!enabled){
			RenderSettings.ambientLight = colorAmbientInsidePlanet;
		}else{
			RenderSettings.ambientLight = originalAmbientLight;
		}
	}

	void Update () {
		if(GameManager.getHasToTurnLightsOffInsidePlanet()){
			if(GameManager.getIsInsidePlanet()){
				enableDirectionalLights(false);
			}else{
				enableDirectionalLights(true);
			}
		}
	}
}
