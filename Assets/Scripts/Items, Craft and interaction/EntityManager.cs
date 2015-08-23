using UnityEngine;
using System.Collections;

public class EntityManager : MonoBehaviour {

	private static EntityManager singleton;

	private static bool isInteractuablePopupActivated = false;

	private static float timerCheckInteractuable = 0f;

	public static float timeBetweenInteractuableChecks = 0.1f;
	public static float distanceToInteract = 3f;

	private static Interactuable closestInteractuable;

	public static EntityManager Instance {
		get{ return singleton ?? (singleton = new GameObject("EntityManager").AddComponent<EntityManager>());}
	}	

	/**************************************************
	 * Gets the interactuable entity closest to the GameObject 
	 * passed by parameter.
	 * 
	 * Returns an Interactuable object. 
	 **************************************************/
	public static Interactuable GetClosestInteractuableEntity(GameObject player)
	{
		Interactuable[] closeInteractEntities;
		closeInteractEntities = GameObject.FindObjectsOfType <Interactuable> ();
		if (closeInteractEntities.Length > 0) {
			Interactuable closest = null;
			Vector3 playerPosition = player.transform.position;
			float previousDistance = Mathf.Infinity;
			foreach (Interactuable interactuableEntity in closeInteractEntities) {
				Vector3 currentPosition = interactuableEntity.transform.position;
				float currentDistance = Vector3.Distance(playerPosition,currentPosition);
				if (currentDistance < previousDistance){
					closest = interactuableEntity;
					previousDistance = currentDistance;
				}
			}
			if(previousDistance < distanceToInteract){

					GUIManager.activateInteractuablePopup();

				return closest.GetComponent<Interactuable>();
			} else { 

					GUIManager.deactivateInteractuablePopup(); 

				return null;
			}
		} else {
			return null;
		}
	}

	void Update(){
		timerCheckInteractuable += Time.deltaTime;
		if(timerCheckInteractuable> timeBetweenInteractuableChecks){
			timerCheckInteractuable = 0f;
			closestInteractuable = GetClosestInteractuableEntity(GameManager.player);
		}
	}

	public  static Interactuable getClosestInteractuable(){
		return closestInteractuable;
	}

}
