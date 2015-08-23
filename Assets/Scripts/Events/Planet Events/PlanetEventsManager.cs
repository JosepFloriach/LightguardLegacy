using UnityEngine;
using System.Collections;

abstract public class PlanetEventsManager : MonoBehaviour {
	
	public bool isEnabled;
	private bool active;
	private bool initialized = false;
	public abstract void informEventActivated (CutsceneIdentifyier identifyier);
	public abstract void initialize ();
	public abstract void isActivated ();
	public abstract void isDeactivated ();

	public virtual void startButtonPressed (){}
	public virtual void planetCleansed (){}
	public virtual void playerDies (){}
	public virtual void firstWaveFinished(){}
	public virtual void secondWaveFinished(){}
	public virtual void playerRespawned(){}
	public virtual void onFadeOutAfterDeath (){}
	public virtual void chargeSpaceJumping(){}

	private bool isInterrupted = false;

	void Start(){
		if(!initialized){
			initialized = true;
			initialize ();
		}
	}

	public void activate(){
		active = true;
		isActivated ();
	}


	public void deactivate(){
		active = false;
		isDeactivated ();
	}

	public void interrupt(){
		isInterrupted = true;
	}
	protected IEnumerator WaitInterruptable(float timeToWait){
		yield return StartCoroutine(WaitInterruptable(timeToWait));
	}
	protected IEnumerator WaitInterruptable(float timeToWait,GameObject dialogue){
		isInterrupted = false;
		float timer = 0f;
		while(timer<timeToWait){
			timer+=Time.deltaTime;
			if(isInterrupted){
				isInterrupted = false; 
				if(dialogue!=null && dialogue.activeSelf){
					dialogue.GetComponent<SpeechBubble>().deactivate(); 
				}
				break;
			}
			yield return null;
		}
	}
}
