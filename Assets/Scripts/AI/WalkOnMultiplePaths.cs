using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WalkOnMultiplePaths : MonoBehaviour {

	public LayerMask layersToFindCollisionEnemies;
	public bool isFrontPath = true;
	public bool takesBothPathways = false;
	public float maximumDistanceSee = 30f;

	private float lastTimeCheckedOtherPaths ;
	private float cooldownRaycastingCheckOtherPaths = 1f;

	private bool isChangingPath = false;
	private float changingTimer = 0f;

	public float centerToExtremesDistance = 0f;

	private List<GameObject> closeOtherEnemies = new List<GameObject>(0);

	IAController ia;


	void OnTriggerEnter (Collider col)
	{
		elementEntersRange (col.gameObject);
	}
	
	void OnCollisionEnter (Collision col)
	{
		//Debug.Log (col.gameObject.tag);
		elementEntersRange (col.gameObject);
	}
	
	void OnTriggerExit(Collider col)
	{
		elementLeavesRange (col.gameObject);
	}
	
	void OnCollisionExit(Collision col)
	{
		elementLeavesRange (col.gameObject);
	}

	void elementEntersRange(GameObject newElement){
		if(newElement.tag == "Enemy"){
			closeOtherEnemies.Add(newElement);
			//Debug.Log(closeOtherEnemies.Count);
		}
	}

	void elementLeavesRange(GameObject element){
		if(element.tag == "Enemy"){
			closeOtherEnemies.Remove(element);
			//Debug.Log(closeOtherEnemies.Count);
		}
	}

	// Use this for initialization
	void Start () {
		ia = GetComponent<IAController> ();
		centerToExtremesDistance = GetComponent<Collider>().bounds.size.z / 2f;
		//Debug.Log (centerToExtremesDistance);
		if(takesBothPathways){
			putMiddlePath();
		}else{
			isChangingPath = true;
		}
		lastTimeCheckedOtherPaths = Random.value * cooldownRaycastingCheckOtherPaths;
	}
	
	// Update is called once per frame
	void Update () {
		if(isChangingPath){
			changePath();
		}
		if(!isChangingPath){
			if(hasToChangePath()){
				isFrontPath = !isFrontPath;
				isChangingPath = true;
			}
		}
	}

	public float getClosestEnemyInFront(){
		float closestEnemyDistance = float.PositiveInfinity;
		foreach(GameObject enemyClose in closeOtherEnemies){
			if(((ia.getIsLookingRight()) && Util.isARightToB(enemyClose,gameObject)) ||
			   (!ia.getIsLookingRight() && !Util.isARightToB(enemyClose,gameObject))){
				if(enemyClose!=null){
					WalkOnMultiplePaths womp = enemyClose.GetComponent<WalkOnMultiplePaths>();
					if((womp.isFrontPath == isFrontPath) || (takesBothPathways || womp.takesBothPathways)){
						float distance = Vector3.Distance(transform.position,enemyClose.transform.position)-centerToExtremesDistance-womp.centerToExtremesDistance;
						if(distance<closestEnemyDistance){
							closestEnemyDistance = distance;
						}
					}
				}


			}
		}
		return closestEnemyDistance;
	}

	public void changePath(){
	
		changingTimer += Time.deltaTime;
		if(changingTimer >= 1f){
			isChangingPath = false;
			changingTimer = 0f;
			takesBothPathways = false;
		}else{
			takesBothPathways = true;
			Vector3 frontPosition = new Vector3(transform.position.x,transform.position.y,Constants.FRONT_PATH_Z_OFFSET);
			Vector3 backPosition = new Vector3(transform.position.x,transform.position.y,Constants.BACK_PATH_Z_OFFSET);

			if (isFrontPath) {transform.position = Vector3.Lerp(backPosition,frontPosition,changingTimer);}
			else{transform.position = Vector3.Lerp(frontPosition,backPosition,changingTimer);}		
		}
	}

	public void putMiddlePath(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, (Constants.FRONT_PATH_Z_OFFSET + Constants.BACK_PATH_Z_OFFSET) / 2f);
	}

	public int ammountOfEnemiesInFront(bool isFrontPath,int jumps){
		float playerAngle = Util.getPlanetaryAngleFromAToB (gameObject, GameManager.player);

		int enemiesBetweenPlayerAndMe = 0;
		foreach(GameObject enemyClose in closeOtherEnemies){
			if(enemyClose!=null){
				WalkOnMultiplePaths womp = enemyClose.GetComponent<WalkOnMultiplePaths>();

				if(womp.isFrontPath == isFrontPath || (womp.takesBothPathways || takesBothPathways)){
					float enemyAngle = Util.getPlanetaryAngleFromAToB(gameObject,enemyClose);
					if(playerAngle<0f && enemyAngle<0f){
						//if they're both negative
						if(playerAngle<enemyAngle){
							enemiesBetweenPlayerAndMe++;
						}
					}else if(playerAngle>0f && enemyAngle>0f){
						if(playerAngle>enemyAngle){
							enemiesBetweenPlayerAndMe++;
						}
					}
				}
			}
		}

		return enemiesBetweenPlayerAndMe;
	}

	public bool canActuallyChangePath(){
		foreach(GameObject enemyClose in closeOtherEnemies){
			if(enemyClose!=null){
				WalkOnMultiplePaths womp = enemyClose.GetComponent<WalkOnMultiplePaths>();
				if((womp.isFrontPath != isFrontPath) && !takesBothPathways){
					//IF they walk on different paths we check if they are gonna collide
					Vector3 distanceBetweenObjects = transform.position - enemyClose.transform.position;
					//We discard the z component
					float distance = new Vector2(distanceBetweenObjects.x,distanceBetweenObjects.y).magnitude;

					if(distance < (centerToExtremesDistance+womp.centerToExtremesDistance)){
						//It's not gonna fit!!! , you Shall not change path
						return false;
					}
				}
			}
		}
		//Nothing went wrong, so you can actually change path
		return true;
	}

	private bool hasToChangePath(){
		if(takesBothPathways){return false;}
		lastTimeCheckedOtherPaths += Time.deltaTime;
		if(lastTimeCheckedOtherPaths>cooldownRaycastingCheckOtherPaths){
			lastTimeCheckedOtherPaths = Random.value*0.1f;
			int frontEnemies = ammountOfEnemiesInFront (true,0);
			int backEnemies = ammountOfEnemiesInFront (false,0);
			
			//Debug.Log("F: "+frontEnemies+" B: "+backEnemies);
			bool canChange = canActuallyChangePath();

			if(canChange){
				if(isFrontPath && backEnemies<frontEnemies){
					return true;
				}else if(!isFrontPath && backEnemies>frontEnemies){
					return true;
				}
			}
			return false;
		}else{
			return false;
		}
	}

	public List<GameObject> getListCloseAllies(){
		return closeOtherEnemies;
	}
	
	public bool IsFrontPath(){
		return isFrontPath;
	}

	public bool getIsChangingPath(){
		return isChangingPath;
	}
}
