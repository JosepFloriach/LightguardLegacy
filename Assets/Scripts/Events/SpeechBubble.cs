using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class SpeechBubble : MonoBehaviour {

	public GameObject speechPosition;
	public float zPosition = -0.5f;
	private float timeItLasts = 1f;
	private GameObject gameObjectToFollow;
	
	protected Text textO;
	bool isActive = false;
	float timer = 0f;
	Vector3 offsetFromObject;
	private bool fadeOut;
	Vector3 cornerToCenter;
	private bool firstFrameSkipped = false;
	private bool calculatedVariablesAfterFirstFrame = false;
	private Vector3 originalScale;

	void Awake(){
		originalScale = transform.localScale;
	}

	public virtual void initialize(string text,GameObject goToFollow,float timeItLasts,bool fadeOut){
		this.fadeOut = fadeOut;
		textO = GetComponentInChildren<Text> ();
		textO.text = text;
		gameObjectToFollow = goToFollow;
		this.timeItLasts = timeItLasts;
		isActive = true;
		offsetFromObject = Vector3.zero;
		timer = 0f;
		GetComponentInChildren<CanvasGroup> ().alpha = 1f;
		transform.localScale = originalScale;

		putInPosition();

		GetComponentInChildren<CanvasGroup> ().alpha = 0f;

		firstFrameSkipped = false;
		calculatedVariablesAfterFirstFrame = false;
	}
	
	protected IEnumerator beat(){
		
		float timer = 0f;
		float beatTime = 0.15f;
		float extraScale = 0.01f;
		Vector3 extraScaleV = new Vector3 (extraScale, extraScale, extraScale);
		
		
		while(timer<beatTime){
			timer+=Time.deltaTime;
			float ratio = timer/beatTime;
			transform.localScale = originalScale + (extraScaleV * ratio);
			yield return null;
		}
		
		timer = 0f;
		while(timer<beatTime){
			timer+=Time.deltaTime;
			float ratio = 1f-(timer/beatTime);
			transform.localScale = originalScale + (extraScaleV * ratio);
			yield return null;
		}
	}

	private void putInPosition(){
		GetComponentInChildren<CanvasGroup> ().alpha = 1f;
	}
	
	void Update(){
		if(isActive){
			timer+=Time.deltaTime;
			if(timer>timeItLasts){
				onFinish();
				deactivate();
			}else{
				if(fadeOut){
					float ratio = timer/timeItLasts;
					GetComponentInChildren<CanvasGroup>().alpha = 1f-ratio;
				}
				putInPosition();
			}
		}
		virtualUpdate ();
	}

	protected virtual void virtualUpdate(){}

	protected abstract void onFinish();

	public void deactivate(){
		onFinish ();
		isActive = false;
	}
}
