using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public bool fixedTime = false;
	float alpha = 0f;
	int dir = 1;

	public float max  = 1f;
	public float min = 0f;
	public float speed = 1f;

	CanvasGroup group;

	// Use this for initialization
	void Start () {
		alpha = min;
		group = GetComponent<CanvasGroup> ();
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
		group.alpha = alpha;

	}
}
