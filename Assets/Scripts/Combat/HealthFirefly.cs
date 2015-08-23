using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthFirefly : MonoBehaviour,AnimationSubscriber {
	public int lifeToGain;
	public float timeToWaitToDisappear = 0.5f;
	public float rotationSpeed;
	public float sinusoidalAmmount = 0.4f;

	private float timer;
	private Vector3 originalPos;
	private Vector3 closestPlanetCenter;
	private bool disappearing;
	private Vector3 originalAngle;
	private bool stuckInTongue = false;
	private AnimationEventBroadcast eventHandler;
	private Vector3 lastAddedSinusoidalmovement;

	public void OnTriggerEnter(Collider collider){
		eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
		if(collider.tag=="Player" && !disappearing){
			GameManager.playerAnimator.SetTrigger("isGrabbingHealth");
			disappearing = true;
			GameManager.player.GetComponent<PlayerController>().gainLife(lifeToGain);
			
			GameManager.audioManager.PlaySound(7);
			
			StartCoroutine("disappear");
		}
	}

	IEnumerator disappear(){
		//GetComponent<Renderer> ().enabled = false;
		yield return new WaitForSeconds (timeToWaitToDisappear);
		transform.position = GameManager.playerController.playerTongueObject.transform.position;
		GameManager.playerController.playerNeckObject.GetComponent<NeckController> ().stopRotating ();
		GameManager.playerController.playerNeckObject.GetComponent<NeckController>().UnlockAngle();
		Destroy (gameObject);
		GameManager.playerAnimator.ResetTrigger("isGrabbingHealth");
	}

	void Start(){

		disappearing = false;
		originalPos = transform.position;
		GameObject[] planets = GravityAttractorsManager.getGravityAttractors ();
		closestPlanetCenter = new Vector3(0f,0f,0f);
		float smallestDistance = float.PositiveInfinity;
		foreach(GameObject planet in planets){
			float distance = Vector3.Distance(planet.transform.position,transform.position);
			if(distance<smallestDistance){
				closestPlanetCenter = planet.transform.position;
				smallestDistance = distance;
			}
		}
		timer = Random.value;
	}
	void Update(){
		timer += Time.deltaTime;
		transform.up = (transform.position - closestPlanetCenter).normalized;
		if(stuckInTongue){
			transform.position = GameManager.playerController.playerTongueObject.transform.position;
		}else{
			if(disappearing){
				GameManager.playerController.playerNeckObject.GetComponent<NeckController>().setObjectivePosition(transform.position);
			}
			transform.RotateAround(closestPlanetCenter,Vector3.forward,Time.deltaTime*rotationSpeed);
			Vector3 sinusoidalIncrease = new Vector3(Mathf.Sin(timer),Mathf.Sin (timer),0f) * sinusoidalAmmount;
			if(lastAddedSinusoidalmovement!=null){
				transform.position -= lastAddedSinusoidalmovement;
				lastAddedSinusoidalmovement = sinusoidalIncrease;
			}
			transform.position += sinusoidalIncrease;
		}
	}
	
	void AnimationSubscriber.handleEvent(string idEvent) {
		switch (idEvent) {
		case "Stick": 
			if(disappearing){
				stuckInTongue = true;
				GameManager.playerController.playerNeckObject.GetComponent<NeckController>().LockAngle();
			}
			break;
		default: 
			break;
		}
	}
	
	string AnimationSubscriber.subscriberName() {
		return  "GrabHealth";	
	}

}
