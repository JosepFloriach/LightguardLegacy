using UnityEngine;
using System.Collections;

public class CorruptionPlane : MonoBehaviour {

	public float speed = 3f;
	public float distance = 45f; 
	public float maxPlayerDistance = 5f;
	private float distanceGone = 0f;
	private float originalSpeed;

	void Awake(){
		originalSpeed = speed;
	}
	
	// Update is called once per frame
	void Update () {

		distanceGone += speed * Time.deltaTime;
		if(distanceGone>=distance){
			//Destroy(gameObject);
			speed = 0f;
		}else if(Vector3.Distance(transform.position,GameManager.player.transform.position)>maxPlayerDistance){
			speed = originalSpeed * 5f;
		}else{
			speed = originalSpeed;
		}
		transform.position += transform.up * speed * Time.deltaTime;
	}

	void OnTriggerEnter(Collider collider){
		if(collider.tag == "Player"){
			GameManager.playerController.kill();
		}
	}
}
