using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InformButtonHighlighted : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,ISelectHandler,IDeselectHandler {

	private bool selected = false;
	private bool pointer = false;

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData){
		GUIManager.informHighlighted (gameObject);
		if(GetComponent<Button> ()!=null){
			GetComponent<Button> ().Select ();
		}
		pointer = true;
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData){
		if(!selected){
			GUIManager.informUnhighlighted(gameObject);
		}
		if(GetComponent<Button> ()!=null){
			GetComponent<Button> ().Select ();
		}
		pointer = false;
	}

	void ISelectHandler.OnSelect(BaseEventData eventData){
		GUIManager.informHighlighted (gameObject);
		selected = true;
	}

	void IDeselectHandler.OnDeselect(BaseEventData eventData){
		if(!pointer){
			GUIManager.informUnhighlighted(gameObject);
		}
		selected = false;
	}

}
