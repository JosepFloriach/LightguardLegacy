using UnityEngine;
using System.Collections;

public class Slash : MonoBehaviour {

	public float timeToLast = 1f;
	public float scaleToGrow = 2f;
	public float speed = 4f;
	Vector3 direction;

	float timer = 0f;
	Material material;

	public void initialize(Vector3 dir){
		direction = dir;
		material = GetComponent<Renderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer>timeToLast){
			Destroy(gameObject);
		}else{
			Vector3 movement = direction.normalized * speed * Time.deltaTime;

			float invertRatio = 1f -( timer/timeToLast);
			transform.position+=movement;
			transform.localScale+=(new Vector3(scaleToGrow,scaleToGrow,scaleToGrow)*Time.deltaTime);
			Color color = new Color(invertRatio,invertRatio,invertRatio,invertRatio) * 0.5f;
			material.SetColor("_TintColor",color);
		}
	}
}
