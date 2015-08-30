using UnityEngine;
using System.Collections;

public class HydraPlatform : MonoBehaviour {

	public Material burningMaterial;
	private Vector3 originalPosition;
	private Material originalMaterial;

	private bool activated = true;

	void Awake(){
		originalPosition = transform.position;
		originalMaterial = GetComponent<MeshRenderer> ().material;
	}

	public void hasBeenTouched(){
		if (activated) {
			StartCoroutine(burnAndGoDown());
			activated = false;
		}
	}

	public void repositionPlatform(){
		GetComponent<MeshRenderer> ().material = originalMaterial;
		StartCoroutine (cleanAndGoUp ());
	}

	IEnumerator cleanAndGoUp(){
		float timer = 0f;
		float time = 1.5f;
		
		while (timer<time) {
			timer+=Time.deltaTime;
			float ratio = timer/time;
			transform.position = Vector3.Lerp(transform.position,originalPosition,ratio);
			yield return null;
		}
		activated = true;
	}

	IEnumerator burnAndGoDown(){
		yield return new WaitForSeconds (1.2f);
		GetComponent<MeshRenderer> ().material = burningMaterial;
		yield return new WaitForSeconds (1.5f);
		float timer = 0f;
		float time = 1.5f;
		Vector3 platformPosition = GetComponent<Renderer>().bounds.center;
		Vector3 direction = platformPosition - transform.position;
		Vector3 objective = originalPosition - (direction.normalized * 1f);

		while (timer<time) {
			timer+=Time.deltaTime;
			float ratio = timer/time;
			transform.position = Vector3.Lerp(originalPosition,objective,ratio);
			yield return null;
		}
	}

	public bool isActivated(){
		return activated;
	}

}
