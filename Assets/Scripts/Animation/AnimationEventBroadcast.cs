using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**Envia los eventos de animacion generados en el mecanim asociado a las animaciones
	subscritas. 
*/
public class AnimationEventBroadcast : MonoBehaviour {

	List<AnimationSubscriber> subList = new List<AnimationSubscriber>();
	Animator animator;
	
	
	void Start() {
		animator = GetComponent<Animator>();
	}
	
			
	public void subscribe (AnimationSubscriber newSub) {
		if (!subList.Contains(newSub)) {
			subList.Add(newSub);
		}
	}
	
	public void unsubscribe (AnimationSubscriber removeSub) {
		subList.Remove(removeSub);
	}

	char[] delimiterChars = { ':' };
	
	/**
	Funcion que dado un string que representa evento destino y mensaje, separados por delimiterChars
	Envia el mensaje a todos los subscriptores. 
	
	TODO: Considerar algun modo para un broadcast masivo.
	*/
	public void informEvent(string msg) {
		StartCoroutine (informSubscribed (msg));
	}

	private IEnumerator informSubscribed(string msg){
		string[] pMsg = msg.Split(delimiterChars);
		if (pMsg.Length >= 2) {
			foreach (AnimationSubscriber sub in subList) {
				if (sub.subscriberName () == pMsg [0]) {
					sub.handleEvent (pMsg [1]);
				}
			}
		}
		yield return null;
	}



}
