using UnityEngine;
using System.Collections;

public class KameOnHitExplosion : MonoBehaviour {

	public float timeToGrow = 0.3f;
	public float timeToWaitWhenGrown = 0.2f;
	public float maxScaleToGrow = 0.5f;
	float timer;
	// Use this for initialization
	void Start () {
		timer = 0f;
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;
		if(timer>=timeToGrow){
			float ratio = (timer-timeToGrow)/timeToWaitWhenGrown;
			Color originalColor = GetComponent<Renderer>().material.GetColor("_TintColor");
			Color newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f-ratio);
			GetComponent<Renderer>().material.SetColor("_TintColor",newColor);
			foreach(Renderer r in GetComponentsInChildren<Renderer>()){
				originalColor = r.material.GetColor("_TintColor");
				newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f-ratio);
				r.material.SetColor("_TintColor",newColor);
			}
			if(timer>=(timeToGrow+timeToWaitWhenGrown)){
				Destroy(gameObject);
			}
		}else{
			float ratio = (timer/timeToGrow) * maxScaleToGrow;
			transform.localScale = new Vector3(ratio,ratio,ratio);
		}
	}
}
