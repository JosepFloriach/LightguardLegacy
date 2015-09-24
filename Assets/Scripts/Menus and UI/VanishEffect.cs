using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class VanishEffect : MonoBehaviour {
	private bool effectActive;
	private bool revertEffectActive;
	private Color originalColor;
	float originalXSize; 
	float originalYSize;
	
	// Use this for initialization
	void Awake () {
		originalColor = GetComponent<Image> ().color;
		effectActive = false;
		originalXSize = transform.localScale.x;
		originalYSize = transform.localScale.y;
	}

	void Update() {
		if (effectActive == true) {
			GetComponent<Image>().CrossFadeAlpha(0, .5f, false);
			transform.localScale = Vector3.Lerp (transform.localScale, new Vector2(0f,0f), Time.deltaTime);
		}

		if (revertEffectActive == true) {
			GetComponent<Image>().CrossFadeColor(originalColor, .5f, true, true);
			transform.localScale = Vector3.Lerp (transform.localScale, new Vector2 (1f, 1f), Time.deltaTime);
		}

		if (transform.localScale.x < 0.01f){
			effectActive = false;
		}

		if (transform.localScale.x >= 1f) {
			revertEffectActive = false; 
		}
	}

	public void activateVanishEffect() {
		effectActive = true;
		revertEffectActive = false; 
	}

	public void deactivateVanishEffect() {
		effectActive = false;
	}

	public void activateRevertEffect() {
		revertEffectActive = true;
		effectActive = false;
	}

	public void deactivateRevertVanishEffect() {
		revertEffectActive = false;
	}
}
