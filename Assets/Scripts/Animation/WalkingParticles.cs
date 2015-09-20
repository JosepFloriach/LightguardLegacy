using UnityEngine;
using System.Collections;

public class WalkingParticles : MonoBehaviour, AnimationSubscriber {

	public ParticleSystem[] walkEffects;
	public int dSteep = 0;
	public Transform bigp;

	// Use this for initialization
	void Start () {
		AnimationEventBroadcast eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
	}
	
	void AnimationSubscriber.handleEvent(string idEvent) {
		if(idEvent == "step"){
		//	transform.up = bigp.up;
		
			//walkEffects[dSteep].Play();
		}
	}
	
	string AnimationSubscriber.subscriberName() {
		return  "Walking";	
	}
	

}
