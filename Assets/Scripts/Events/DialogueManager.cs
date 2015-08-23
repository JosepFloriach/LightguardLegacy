using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour {

	public GameObject prefabDialogue;
	public GameObject prefabExpression;


	private List<GameObject> dialoguesPool;
	private List<GameObject> expressionPool;

	void Start(){
		dialoguesPool = new List<GameObject> (0);
		expressionPool = new List<GameObject> (0);
		GameManager.registerDialogueManager (this);
	}

	public GameObject getDialogue(){
		if(dialoguesPool.Count==0){
			GameObject newDialogue = Instantiate(prefabDialogue) as GameObject;
			return newDialogue;
		}else{
			//Reset the dialogue
			GameObject dialogue = dialoguesPool[dialoguesPool.Count-1];
			dialoguesPool.Remove(dialogue);
			dialogue.SetActive (true);
			return dialogue;
		}
	}

	public GameObject getExpression(){
		if(expressionPool.Count==0){
			GameObject newExpression = Instantiate(prefabExpression) as GameObject;
			return newExpression;
		}else{
			//Reset the expression
			GameObject expression = expressionPool[expressionPool.Count-1];
			expressionPool.Remove(expression);
			expression.SetActive (true);
			return expression;
		}
	}

	public void dialogueFinished(GameObject dialogue){
		dialoguesPool.Add (dialogue);
		dialogue.SetActive (false);
	}

	public void expressionFinished(GameObject expression){
		expressionPool.Add (expression);
		expression.SetActive (false);
	}

}
