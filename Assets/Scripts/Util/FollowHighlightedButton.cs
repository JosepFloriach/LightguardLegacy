using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;
public class FollowHighlightedButton : MonoBehaviour {

	public float distance;
	public float speed = 3f;
	public bool lerpABit;
	public float lerpCoeficient = 0.005f;
	public GameObject spline;
	private bool isActive = false;
	private GameObject highlightedObject;
	private Vector3 originalScale;
	public float scaleToGrow;
	public float timeToGrow;
	private float deltaTime;
	private float lastTime;
	// Use this for initialization
	void Start () {
		GUIManager.registerHightlightFollower (gameObject);
		originalScale = spline.transform.localScale;
		transform.parent = GameManager.mainCamera.transform;
		lastTime = Time.realtimeSinceStartup;
	}

	public void informHighlightedObject(GameObject highlightedObject){
		this.highlightedObject = highlightedObject;
	}

	public void informUnhighlightedObject(GameObject highlightedObject){
		//this.highlightedObject = null;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		deltaTime = Time.realtimeSinceStartup - lastTime;
		lastTime = Time.realtimeSinceStartup;
			if(GUIManager.showLeafsInActualMenu() && highlightedObject!=null){
				if(!isActive){
					StartCoroutine("unGrow");
				}

				Ray ray = GameManager.mainCamera.GetComponent<Camera> ().ScreenPointToRay (highlightedObject.transform.position);

				Vector3 position = ray.GetPoint(distance);
				float speedByTime = (speed * deltaTime);
				if(!isActive){
					transform.position = position;
				}else{
					Vector3 direction = position - transform.position;
					//transform.position += direction.normalized * speedByTime;
					transform.position = Vector3.Lerp(transform.position,position,lerpCoeficient);
				}
				transform.rotation = GameManager.mainCamera.transform.rotation;
				isActive = true;

			}else{
				if(isActive){
					StartCoroutine("grow");
				}
				isActive = false;
			}
	}

	IEnumerator grow(){
		float timer = 0f;
		while(timer<timeToGrow){
			timer+=deltaTime;
			float ratio = timer/timeToGrow;
			float newScale = originalScale.x+ ((scaleToGrow - originalScale.x) * ratio);
			spline.transform.localScale = new Vector3(newScale,newScale,newScale);
			yield return null;
		}
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			r.enabled = false;
		}
	}

	IEnumerator unGrow(){
		float timer = 0f;
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			r.enabled = true;
		}
		while(timer<timeToGrow){
			timer+=deltaTime;
			float inverRatio = 1f - (timer/timeToGrow);
			float newScale = originalScale.x + ((scaleToGrow - originalScale.x) * inverRatio);
			spline.transform.localScale = new Vector3(newScale,newScale,newScale);
			yield return null;
		}
		spline.transform.localScale = originalScale;
	}
}
