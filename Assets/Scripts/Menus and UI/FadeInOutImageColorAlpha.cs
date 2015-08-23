using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeInOutImageColorAlpha : MonoBehaviour {
	
	public bool fixedTime = false;
	float alpha = 0f;
	int dir = 1;
	
	public float max  = 1f;
	public float min = 0f;
	public float speed = 1f;
	
	Image group;
	
	// Use this for initialization
	void Start () {
		alpha = min;
		group = GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		float time = Time.deltaTime;
		if (fixedTime) {
			time = Time.fixedDeltaTime;
		}
		alpha += dir * speed * time;
		
		if(alpha >= max || alpha <= min){
			dir*=-1;
		}
		alpha = Mathf.Clamp01 (alpha); 
		Color color = group.color;
		color.a = alpha;
		group.color = color;
		
	}
}
