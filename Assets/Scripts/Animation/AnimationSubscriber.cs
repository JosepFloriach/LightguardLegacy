using UnityEngine;
using System.Collections;
/**
Usado en conjunto con AnimationEventBroadcast para enviar mensajes desde la animacion del mecanim
hasta los controladores de los eventos individuales de animacion.
*/
public interface AnimationSubscriber {
//Nombre del evento suscrito, intentar usar el nombre de la animacion
	string subscriberName();
//Se encarga de la logica del evento. 
	void handleEvent(string idEvent);
}
