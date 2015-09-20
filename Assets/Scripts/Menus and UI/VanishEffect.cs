using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class VanishEffect : MonoBehaviour {
	private bool effectActive;
	float basex; 
	float basey;
	float maxSize = 1.5f;
	float speed = 0.2f;
	float direction = 1f; 

	// Use this for initialization
	void Awake () {
		effectActive = false;
		basex = transform.localScale.x;
		basey = transform.localScale.y;
		direction = 1f; 
	}

	void Update() {
		if (effectActive == true) {
			GetComponent<Image>().CrossFadeAlpha(0, .5f, false);
			transform.localScale = Vector3.Lerp (transform.localScale, new Vector3(0f,0f), Time.deltaTime);
		}
		if (transform.localScale.x < 0.1f) {
			effectActive = false;
		}
	}

	public void activateVanishEffect() {
		effectActive = true;
	}

	public void deactivateVanishEffect() {
		effectActive = false;
	}
}
