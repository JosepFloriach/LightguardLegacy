using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour {
	
	public GameObject speechPosition;
	public Color bubbleColor;

	public GameObject createNewDialogue(string text,float timeToLast,bool bouncing,bool fadeOut){
		GameObject newDialogue = GameManager.dialogueManager.getDialogue ();
		newDialogue.transform.position =  speechPosition.transform.position;
		newDialogue.transform.parent = null;
		if (newDialogue.GetComponentInChildren<RawImage> () != null) {
			newDialogue.GetComponentInChildren<RawImage> ().color = bubbleColor;
		}
		newDialogue.GetComponent<Dialogue> ().speechPosition = speechPosition;
		//BOUNCING DISABLED
		newDialogue.GetComponent<Dialogue> ().initialize (text, gameObject, timeToLast,false,fadeOut);

		newDialogue.transform.up = transform.up;

		return newDialogue;
	}

	public GameObject createNewExpression(string text,float timeToLast,bool bouncing,bool fadeOut){
		GameObject newExpression = GameManager.dialogueManager.getExpression ();
		newExpression.transform.position =  speechPosition.transform.position;
		newExpression.transform.parent = null;
		newExpression.GetComponentInChildren<Image> ().color = bubbleColor;
		newExpression.GetComponent<Expression> ().speechPosition = speechPosition;
		newExpression.GetComponent<Expression> ().initialize (text, gameObject, timeToLast,bouncing,fadeOut);

		newExpression.transform.up = transform.up;

		return newExpression;
	}

	void Update(){
	}

}
