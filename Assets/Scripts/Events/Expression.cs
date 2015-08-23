using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Expression : SpeechBubble {

	protected override void onFinish(){
		GameManager.dialogueManager.expressionFinished (gameObject);
	}
}
