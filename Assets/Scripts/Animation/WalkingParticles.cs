using UnityEngine;
using System.Collections;

public class WalkingParticles : MonoBehaviour, AnimationSubscriber {

	ParticleSystem dust;

	// Use this for initialization
	void Start () {
		AnimationEventBroadcast eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
		
		dust = this.GetComponent<ParticleSystem>();
	}
	
	void AnimationSubscriber.handleEvent(string idEvent) {
		if(idEvent == "step"){
			dust.Play();
		
		}
	}
	
	string AnimationSubscriber.subscriberName() {
		return  "Walking";	
	}
	

}
