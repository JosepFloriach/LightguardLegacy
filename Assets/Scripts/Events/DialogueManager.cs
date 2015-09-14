using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TextureDialogue {BigPappada,LittleG,Whiteheart,Mundus}

public class DialogueManager : MonoBehaviour {

	public GameObject prefabDialogue;
	public GameObject prefabExpression;

	public Texture bigPTextureR, littleGTextureR, WhiteheartTextureR, MundusTextureR;
	public Texture bigPTextureL, littleGTextureL, WhiteheartTextureL, MundusTextureL;

	private List<GameObject> dialoguesPool;
	private List<GameObject> expressionPool;

	void Start(){
		dialoguesPool = new List<GameObject> (0);
		expressionPool = new List<GameObject> (0);
		GameManager.registerDialogueManager (this);
	}

	public Texture getTextureDialogue(TextureDialogue t,bool isRight){
		if(t.Equals(TextureDialogue.BigPappada)){
			if(isRight){
				return bigPTextureR;
			}else{
				return bigPTextureL;
			}
		}else if(t.Equals(TextureDialogue.LittleG)){
			if(isRight){
				return littleGTextureR;
			}else{
				return littleGTextureL;
			}
		}else if(t.Equals(TextureDialogue.Mundus)){
			if(isRight){
				return MundusTextureR;
			}else{
				return MundusTextureL;
			}
		}else if(t.Equals(TextureDialogue.Whiteheart)){
			if(isRight){
				return WhiteheartTextureR;
			}else{
				return WhiteheartTextureL;
			}
		}
		return null;
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
