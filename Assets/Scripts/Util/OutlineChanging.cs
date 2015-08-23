using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutlineChanging : MonoBehaviour {

	private Color outlineColor;

	private List<Material> materialsList;
	private Color[] originalColors;

	public void setOutlineColor(Color color){
		foreach(Material material in materialsList){
			if(material.HasProperty("_OutlineColor")){
				material.SetColor("_OutlineColor",color);
			}
		}
	}

	public void setMainColor(Color color){
		foreach(Material material in materialsList){
			if(material.HasProperty("_Color")){
				material.SetColor("_Color",color);
			}
		}
	}

	public void setMainColorAndLerpBackToOriginal(Color color,float time){
		setMainColor (color);
		StartCoroutine (changeColorBackToOriginalOverTime (color, time));
	}

	private IEnumerator changeColorBackToOriginalOverTime(Color color,float time){
		float timer = 0f;
		while (timer<time){
			timer+=Time.deltaTime;
			float ratio = timer/time;
			for(int i = 0;i<materialsList.Count;i++){
				if(materialsList[i].HasProperty("_Color")){
					materialsList[i].SetColor("_Color",Color.Lerp(color,originalColors[i],ratio));
				}
			}
			yield return null;
		}
	}

	public void resetMainColor(){
		for(int i = 0;i<materialsList.Count;i++){
			if(materialsList[i].HasProperty("_Color")){
				 materialsList[i].SetColor("_Color",originalColors[i]);
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
		originalColors = new Color[materialsList.Count];
		for(int i = 0;i<materialsList.Count;i++){
			if(materialsList[i].HasProperty("_Color")){
				originalColors[i] = materialsList[i].GetColor("_Color");
			}
		}
	}
}
