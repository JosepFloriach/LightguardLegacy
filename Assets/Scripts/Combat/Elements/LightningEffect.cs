using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningEffect : MonoBehaviour {

	public int damage;
	public int elementsCanJumpTo = 3;
	public float maxDistanceBetweenElements = 20f;
	public float speed = 10f;
	public float timeToWaitAfterEffectFinished = 0.3f;
	public float proportionJaggedness = 0.8f;

	public GameObject[] trails;

	private int jumpsDone = 0;
	


	private bool finished;
	private List<GameObject> elementsGoneThroughLightning;
	private Vector3 actualPosition;

	private GameObject actualObjective;

	public void initialize(GameObject startLightningElement){

		elementsGoneThroughLightning = new List<GameObject> (0);
		elementsGoneThroughLightning.Add (startLightningElement);
		finished = false;
		jumpsDone = 0;
		startLightningElement.GetComponent<IAController> ().getHurt (damage,actualPosition,true);
		actualPosition = startLightningElement.GetComponent<Rigidbody> ().worldCenterOfMass;
		foreach(GameObject trail in trails){
			trail.transform.position = actualPosition;
		}
		GameObject objective = findCloseEnemies ();
		if (objective != null) {
			actualObjective = objective;
		}

	}
	
	void Update(){
		//Advance
		float distanceToMove = speed * Time.deltaTime;

		if(actualObjective!=null){
			if (Vector3.Distance (trails[0].transform.position,actualObjective.GetComponent<Rigidbody>().worldCenterOfMass) < distanceToMove) {
				if(jumpsDone<elementsCanJumpTo){
					actualObjective.GetComponent<IAController>().getHurt(damage,actualPosition,true);
					foreach(GameObject trail in trails){
						trail.transform.position = actualObjective.GetComponent<Rigidbody> ().worldCenterOfMass;
					}
					jumpsDone++;
					if(jumpsDone>=elementsCanJumpTo){
						StartCoroutine("destroyAfterTime");
					}else{
						GameObject objective = findCloseEnemies ();
						if (objective != null) {
							actualObjective = objective;
						}
					}
				}
			}else{
				//Move the distance
				foreach(GameObject trail in trails){
					Vector3 direction = (actualObjective.GetComponent<Rigidbody>().worldCenterOfMass - trail.transform.position).normalized;
					Vector3 randomUp = actualObjective.transform.up.normalized;
					if(Random.value>=0.5f){
						randomUp *=-1f;
					}
					randomUp *= Random.value;
					Vector3 movement = (direction * (1f-proportionJaggedness) * distanceToMove) + (randomUp * proportionJaggedness * distanceToMove);
					trail.transform.position +=movement;
				}
			}
		}else{
			StartCoroutine("destroyAfterTime");
		}

	}

	IEnumerator destroyAfterTime(){
		yield return new WaitForSeconds(timeToWaitAfterEffectFinished);
		Destroy(gameObject);
	}

	private GameObject findCloseEnemies(){
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		List<GameObject> enemiesInRange = new List<GameObject> (0);
		foreach(GameObject enemy in enemies){
			if(Vector3.Distance(enemy.transform.position,actualPosition)<=maxDistanceBetweenElements){
				if(!elementsGoneThroughLightning.Contains(enemy)){
					elementsGoneThroughLightning.Add(enemy);
					return enemy;
				}
			}
		}
		return null;
	}
}
