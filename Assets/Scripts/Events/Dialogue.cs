using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Dialogue : SpeechBubble {

	protected override void onFinish(){
		GameManager.dialogueManager.dialogueFinished (gameObject);
	}
}
