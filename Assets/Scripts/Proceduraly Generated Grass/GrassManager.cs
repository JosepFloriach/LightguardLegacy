using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GrassManager : MonoBehaviour {

	public List<Material> materials;

	void Start(){
		GameManager.registerGrassManager (this);
	}

	public void changeMaterialColors(Color color){
		foreach(Material material in materials){
			material.SetColor("_CorruptedColor",color);
		}
	}

	public void setYCutout(float ycutout){
		foreach(Material material in materials){
			material.SetFloat("_YCutOut",ycutout);
		}
	}
}
