using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Dialogue : SpeechBubble {

	public RawImage leftImage;
	public RawImage rightImage;

	private string textToPut = "";
	private string textPut = "";
	private float writeTimer;

	protected override void onFinish(){
		GameManager.dialogueManager.dialogueFinished (gameObject);
	}

	public void initialize(string text,GameObject goToFollow,float timeItLasts,bool fadeOut,TextureDialogue t,bool isRight){
		base.initialize (text,goToFollow,timeItLasts,fadeOut);
		textToPut = text;
		textPut = "";
		if (isRight) {
			leftImage.enabled = false;
			rightImage.enabled = true;
			rightImage.texture = GameManager.dialogueManager.getTextureDialogue (t);
		} else {
			rightImage.enabled = false;
			leftImage.enabled = true;
			leftImage.texture = GameManager.dialogueManager.getTextureDialogue (t);
		}
		textO.text = "";

		writeTimer = 0f;
	}

	protected override void virtualUpdate(){
		if (textPut.Length < textToPut.Length) {
			writeTimer+=Time.deltaTime;
			if(writeTimer>Constants.DIALOGUE_PRINT_SPEED){
				writeTimer = 0f;
				textPut = textToPut.Substring(0,textPut.Length+1);
				textO.text = textPut;
			}
		}
	}
}
