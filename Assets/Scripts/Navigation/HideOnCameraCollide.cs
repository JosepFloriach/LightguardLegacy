using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HideOnCameraCollide : MonoBehaviour {

	List<MeshRenderer> meshRenderers = new List<MeshRenderer> (0);
	public bool fade = true;

	void Start(){
		MeshRenderer m = GetComponent<MeshRenderer> ();
		if (m != null) {
			meshRenderers.Add (m);
		}
		MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer> ();
		if (childRenderers != null) {
			meshRenderers.AddRange(childRenderers);
		}

	}
	void OnTriggerEnter(Collider c){
		if (c.gameObject.tag == "MainCamera") {
			enableRenderers(false);
		}
	}

	void OnTriggerExit(Collider c){
		if (c.gameObject.tag == "MainCamera") {
			enableRenderers(true);
		}
	}

	void enableRenderers(bool enable){
		foreach (MeshRenderer m in meshRenderers) {
			if(!enable){
				if(fade){
					m.material.SetFloat("_AbsoluteAlpha",0.4f);
					m.material.SetColor("_OutlineColor",Color.clear);
				}else{
					m.enabled = false;
				}
			}else{
				if(fade){
					m.material.SetFloat("_AbsoluteAlpha",1f);
					m.material.SetColor("_OutlineColor",Color.black);
				}else{
					m.enabled = true;
				}
			}
		}
	}
}
