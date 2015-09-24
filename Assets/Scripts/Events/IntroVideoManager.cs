using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IntroVideoManager : MonoBehaviour {

	public bool interruptable = true;
	private bool hasBeenDeactivated = false;
	private MovieTexture mt;
	private Material mat;
	private float timeToWaitTillStart = 0.5f;
	private bool hasStarted = false;
	private float timer = 0f;
	// Use this for initialization
	void Awake () {
		timer = 0f;
		RawImage ri = this.GetComponent<RawImage>();
		mat = ri.material;
		mat.SetFloat("_DissolveThreshold",1f);
		mat.SetColor ("_Color", Color.black);
		mt = mat.mainTexture as MovieTexture;
		mt.Play();
		//GameManager.audioManager.PlaySong( mt.audioClip);
	}
	
	// Update is called once per frame
	void Update () {
		timer+=Time.deltaTime;
		if(timer>timeToWaitTillStart && !hasStarted){
			mat.SetColor ("_Color", Color.white);
			hasStarted = true;
			mt.Play();
		}

		if (interruptable && Input.anyKeyDown && !hasBeenDeactivated && hasStarted) {
			hasBeenDeactivated = true;
			StartCoroutine(deactivateIntroScene());
		}

		if(!mt.isPlaying && !hasBeenDeactivated && hasStarted){
			hasBeenDeactivated = true;
			StartCoroutine(deactivateIntroScene());
		}
		
	}

	public bool isFinished(){
		return (!mt.isPlaying || isFading) && hasStarted;
	}

	bool isFading = false;

	IEnumerator deactivateIntroScene(){
		float timer = 0f;
		float timeItLasts = 1f;
		isFading = true;
		GUIManager.fadeOutChangeMenuFadeIn(Menu.MainMenu);
		GameManager.audioManager.playSong(0);
		while(timer<timeItLasts){
			timer+=Time.deltaTime;
			float ratio = 1f-(timer/timeItLasts);
			mat.SetFloat("_DissolveThreshold",ratio);
			yield return null;
		}
		isFading = false;
		mt.Stop ();
		yield return null;
	}
}
