using UnityEngine;
using System.Collections;

public class Meteorite : MonoBehaviour {

	public float timeToSetZtoZero = 2f;
	private Vector3 direction;
	private float speed;
	private float timerZ = 0f;
	private float startingZ ;
	private float timeItLasts = 0f;
	private float timer = 0f;

	public void initialize(Vector3 direction,float speed,float timeItLasts){
		this.direction = direction;
		this.direction.z = 0f;
		this.speed = speed;
		this.timeItLasts = timeItLasts;
		startingZ = transform.position.z;

	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer>timeItLasts){
			Destroy(gameObject);
		}

		if(timerZ<timeToSetZtoZero){
			timerZ += Time.deltaTime;
			float invertRatio = 1f-(timerZ/timeToSetZtoZero);
			Vector3 position = transform.position;
			position.z = invertRatio * startingZ;
			transform.position = position;
		}

		transform.position += direction.normalized * Time.deltaTime * speed;
	}
}
