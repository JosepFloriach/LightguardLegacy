using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dissolve : MonoBehaviour {

	private Color outlineColor;
	
	private List<Material> materialsList;
	
	public void setDisolution(float disolution){
		foreach(Material material in materialsList){
			if(material.HasProperty("_DissolveThreshold")){
				material.SetFloat("_DissolveThreshold",disolution);
			}
		}
	}
	
	void Start(){
		materialsList = new List<Material> (0);
		Renderer rend = GetComponent<Renderer> ();
		if (rend != null) {
			Material[] materials = rend.materials;
			materialsList.AddRange (materials);
		}
		foreach(Renderer renderer in GetComponentsInChildren<Renderer>()){
			materialsList.AddRange(renderer.materials);
		}
	}
}
