using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackElementsManager : MonoBehaviour{
	public GameObject[] elementsGO;

	public static List<Element> elements = new List<Element>(0);

	void Start(){
		foreach(GameObject elementGO in elementsGO){
			Element element = (Instantiate(elementGO) as GameObject).GetComponent<Element>();
			elements.Add(element);
		}
	}

	public static Element getElement(ElementType eType){
		foreach(Element element in elements){
			if(element.getType().Equals(eType)){
				return element;
			}
		}
		return null;
	}
}
