using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class SpeechBubble : MonoBehaviour {

	public GameObject BubbleStartPosition;
	public float zPosition = -0.5f;
	private float timeItLasts = 1f;
	private GameObject gameObjectToFollow;
	
	private Text textO;
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
	public virtual void initialize(string text,GameObject goToFollow,float timeItLasts,bool bouncingIn,bool fadeOut){
		this.fadeOut = fadeOut;
		textO = GetComponentInChildren<Text> ();
		textO.text = text;
		gameObjectToFollow = goToFollow;
		this.timeItLasts = timeItLasts;
		isActive = true;
		transform.parent = gameObjectToFollow.transform; 
		offsetFromObject = transform.localPosition;
		transform.parent = null;
		timer = 0f;
		GetComponentInChildren<CanvasGroup> ().alpha = 1f;
		/*if(originalScale==null){
			originalScale = transform.localScale;
		}else{*/
			transform.localScale = originalScale;

		if(bouncingIn){
			StartCoroutine ("beat");
		}

		putInPosition();

		GetComponentInChildren<CanvasGroup> ().alpha = 0f;

		firstFrameSkipped = false;
		calculatedVariablesAfterFirstFrame = false;
	}
	
	protected IEnumerator beat(){
		
		float timer = 0f;
		float beatTime = 0.15f;
		float extraScale = 0.2f;
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

		if(firstFrameSkipped){
			firstFrameSkipped = true;
		}else if(!calculatedVariablesAfterFirstFrame){
			calculatedVariablesAfterFirstFrame = true;
			GetComponentInChildren<CanvasGroup> ().alpha = 1f;
			Vector3[] corners = new Vector3[4];
			gameObject.transform.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>().GetWorldCorners(corners);
			Vector3 leftBottomPosition = corners [0];
			cornerToCenter = transform.position - leftBottomPosition;
		}
		transform.parent = gameObjectToFollow.transform;
		transform.localPosition = offsetFromObject;
		transform.parent = null;
		float originalZ = transform.position.z;
		float zDifference = zPosition - originalZ;
		Vector3 directionCamera = GameManager.mainCamera.transform.position - transform.position;
		transform.position -= (directionCamera*zDifference);
		transform.position += cornerToCenter;
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
				//transform.position = new Vector3(transform.position.x,transform.position.y,-0.5f);
				transform.up = gameObjectToFollow.transform.up;
				transform.rotation = Quaternion.LookRotation(/*transform.position-GameManager.mainCamera.transform.position*/ Vector3.forward,GameManager.mainCamera.transform.up);
			}
		}
	}

	protected abstract void onFinish();

	public void deactivate(){
		onFinish ();
		isActive = false;
	}

	void Start(){
		originalScale = transform.localScale;
	}

}
