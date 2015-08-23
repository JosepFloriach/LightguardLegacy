using UnityEngine;
using System.Collections;

public class CameraFollowingPlayer : MonoBehaviour {


	public float distanceCameraOnSpaceJump = 100;
	public float distanceCameraOnSmallPlanet = 70;
	public float distanceCameraOnCleansePlanet = 40;
	public float upMultiplyierWithAngle = 2.5f;
	public float upMultiplyierWithoutAngle = 1.2f;

	public float originalUpMultiplyierWithAngle;
	public float lerpMultiplyierZPosition = 4f;
	public float minimumUpDistanceOnStartLerpingXAngle = 1f;
	public float xAngle = 21f;
	public float lerpMultiplyierXAngle = 0.25f;
	public float lerpMultiplyierUp = 4.5f;
	public float lerpMultiplyierPos = 5f;
	private float objectiveZ;
	private float originalZ;

	private GameObject objective;
	private GameObject lastObjective;
	private bool isChangingObjective;
	private float timerChangingObjective;
	private float timeItTakesToChangeObjective = 1f;

	private bool followingObjective;
	private float originalAngleX;
	private float originalMultiplyierUp;
	private float originalMultiplyierPos;
	private float originalMultiplyierZ;
	private bool isShaking = false;
	private bool isStoppingShaking = false;
	//private Vector3 shakeDisplacement;
	//private Vector3 displacementDirection;
	private float shake_intensity = 0f;
	private float shake_decay = 0f;

	private Vector3 staticUp;

//Usado para controlar la camara en la zona de la pagoda
	public bool regularMode = false;


	float timer = 0f;
	float timerZPosition = 0f;

	void Awake(){
		GameManager.registerMainCamera (gameObject);
	}


	// Use this for initialization
	void Start () {
		originalUpMultiplyierWithAngle = upMultiplyierWithAngle;
		originalZ = transform.position.z;
		objectiveZ = originalZ;
		followingObjective = false;

		originalAngleX = xAngle;
		originalMultiplyierUp = lerpMultiplyierUp;
		originalMultiplyierPos = lerpMultiplyierPos;
		originalMultiplyierZ = lerpMultiplyierZPosition;
	}

	void regularPosition() {
		timerZPosition += Time.deltaTime;
		if(objective==null){
			objective = GameManager.player;
		}
		
		Vector3 objectiveUp;
		Vector3 objectivePosition;
		Vector3 objectiveVectorZ;
		
		objectiveUp = staticUp.normalized;
		objectivePosition = new Vector3 (objective.transform.position.x, objective.transform.position.y,transform.position.z);
		
		objectivePosition += objective.transform.up*upMultiplyierWithoutAngle;
		
		Vector3 newUp = Vector3.Lerp (transform.up, objectiveUp,Time.deltaTime * lerpMultiplyierUp);
		Vector3 rightWithoutZ = new Vector3 (transform.right.x, transform.right.y, 0f).normalized;
		Vector3 newForward = Quaternion.AngleAxis(90,rightWithoutZ) * newUp;
		transform.rotation = Quaternion.LookRotation(newForward,newUp);
		
		
		objectiveVectorZ = new Vector3 (objectivePosition.x, objectivePosition.y, objectiveZ);
		objectivePosition = Vector3.Lerp (objectivePosition, objectiveVectorZ, timerZPosition * lerpMultiplyierZPosition );
		
		transform.position = Vector3.Lerp (transform.position, objectivePosition, Time.deltaTime * lerpMultiplyierPos);

		if(isShaking){
			if(shake_intensity > 0){
				Vector3 displacement = Random.insideUnitSphere * shake_intensity;
				displacement.z = 0f;
				transform.position = transform.position + displacement;
				if(isStoppingShaking){
					shake_intensity -= shake_decay;
				}
			}else{
				isShaking = false;
			}
		}


	}

	public void setUpMultiplyierWithAngle(float newUpMultiplyierWithAngle){
		upMultiplyierWithAngle = newUpMultiplyierWithAngle;
	}

	public void resetUpMultiplyierWithAngle(){
		upMultiplyierWithAngle = originalUpMultiplyierWithAngle;
	}

	public void setStaticUp(Vector3 newUp) {
		staticUp = newUp;
	}

	void updatePosition(){
		timerZPosition += Time.deltaTime;
		if(objective==null){
			objective = GameManager.player;
		}

		Vector3 objectiveUp;
		Vector3 objectivePosition;
		Vector3 objectiveVectorZ;
		//If we are changing objectives we calculate the appropiate rotation around the planet
		if(isChangingObjective && GameManager.playerSpaceBody.getClosestPlanet()!=null && GameManager.playerSpaceBody.getClosestPlanet().GetComponent<Planet>().centerCameraOnLand){
			timerChangingObjective +=Time.deltaTime;
			if(timerChangingObjective<timeItTakesToChangeObjective){
				float ratio = timerChangingObjective/timeItTakesToChangeObjective;
				Vector3 lastObjectDirection = lastObjective.transform.position - GameManager.playerSpaceBody.getClosestPlanet().gameObject.transform.position;
				lastObjectDirection.z = 0f;
				Vector3 newObjectiveDirection = objective.transform.position - GameManager.playerSpaceBody.getClosestPlanet().gameObject.transform.position;
				newObjectiveDirection.z = 0f;
				float newMagnitude = ((newObjectiveDirection.magnitude - lastObjectDirection.magnitude)*ratio)+lastObjectDirection.magnitude;
				float angle = (Util.getAngleFromVectorAToB(newObjectiveDirection,lastObjectDirection) * ratio);

				Vector3 newDirection = (((Quaternion.AngleAxis(angle,Vector3.forward)*lastObjectDirection).normalized)*newMagnitude);
				objectivePosition = GameManager.playerSpaceBody.getClosestPlanet().gameObject.transform.position + newDirection;
				objectivePosition.z = transform.position.z;
				objectiveUp = newDirection.normalized;
			}else{
				isChangingObjective = false;
				objectiveUp = new Vector3(objective.transform.up.x,objective.transform.up.y,0f).normalized;
				objectivePosition = new Vector3 (objective.transform.position.x, objective.transform.position.y,transform.position.z);
			}
		}else{
			objectiveUp = new Vector3(objective.transform.up.x,objective.transform.up.y,0f).normalized;
			objectivePosition = new Vector3 (objective.transform.position.x, objective.transform.position.y,transform.position.z);
		}


		objectiveVectorZ = new Vector3 (objectivePosition.x, objectivePosition.y, objectiveZ);
		objectivePosition = Vector3.Lerp (objectivePosition, objectiveVectorZ, timerZPosition * lerpMultiplyierZPosition );
		Vector3 rightWithoutZ = new Vector3 (transform.right.x, transform.right.y, 0f).normalized;

		//Modifying objective up
		if (!GameManager.player.GetComponent<PlayerController> ().getIsSpaceJumping () && !GameManager.playerController.getIsChargingSpaceJump() && !GameManager.getIsInsidePlanet() && GameManager.playerSpaceBody.getClosestPlanet() != null && GameManager.playerSpaceBody.getClosestPlanet().GetComponent<Planet>().centerCameraOnLand) {
			if(Vector3.Distance(objectiveUp,transform.up)<=minimumUpDistanceOnStartLerpingXAngle){
				Vector3 objectiveUpRotated = (Quaternion.AngleAxis (xAngle, rightWithoutZ) * objectiveUp);
				timer+=Time.deltaTime;
				objectiveUp = Vector3.Lerp(objectiveUp,objectiveUpRotated,timer * lerpMultiplyierXAngle);
			}
			objectivePosition += objective.transform.up*upMultiplyierWithAngle;
		}else{
			timer = 0f;
			objectivePosition += objective.transform.up*upMultiplyierWithoutAngle;
		}

		Vector3 newUp;
		SpaceGravityBody playerGravityBody = GameManager.player.GetComponent<SpaceGravityBody> ();
		if ((!playerGravityBody.getUsesSpaceGravity())) {
			newUp = Vector3.Lerp (transform.up, objectiveUp,Time.deltaTime * lerpMultiplyierUp);
		}else{
			newUp = transform.up;
		}
		Vector3 newForward = Quaternion.AngleAxis(90,rightWithoutZ) * newUp;
		transform.rotation = Quaternion.LookRotation(newForward,newUp);
		transform.position = Vector3.Lerp (transform.position, objectivePosition, Time.deltaTime * lerpMultiplyierPos);

		if(isShaking){
			if(shake_intensity > 0){
				Vector3 displacement = Random.insideUnitSphere * shake_intensity;
				displacement.z = 0f;
				transform.position = transform.position + displacement;
				if(isStoppingShaking){
					shake_intensity -= shake_decay;
				}
			}else{
				isShaking = false;
			}
		}


		float zProportion = Mathf.Abs (transform.position.z - originalZ) / Mathf.Abs (distanceCameraOnSpaceJump - originalZ);
		GameManager.setGrassPorcentualLevel (zProportion);



	}

	public void setCameraShaking(){
		isShaking = true;
		shake_decay = 0.002f;
		shake_intensity = 0.05f;
		isStoppingShaking = false;
	}
	public void stopCameraShaking(){
		isStoppingShaking = true;
	}

	public void returnOriginalZ(){
		timerZPosition = 0f;
		objectiveZ = originalZ;
	}

	public void setObjectiveZCameraOnSpaceJump(){
		timerZPosition = 0f;
		objectiveZ = -distanceCameraOnSpaceJump;
	}

	public void setObjectiveZCameraCleansePlanet(){
		timerZPosition = 0f;
		objectiveZ = -distanceCameraOnCleansePlanet;
	}

	public void setObjectiveZCameraSmallPlanet(){
		timerZPosition = 0f;
		objectiveZ = -distanceCameraOnSmallPlanet;
	}

	public void setObjectiveZ(float newObjectiveZ){
		timerZPosition = 0f;
		objectiveZ = -newObjectiveZ;
	}

	public float getObjectiveZ(){
		return objectiveZ;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(GameManager.isCameraLockedToPlayer){
			if (regularMode) regularPosition();
			else updatePosition ();
		}
	}

	public void resetPosition(){
		if(objective==null){
			objective = GameManager.player;
		}
		GravityBody playerGravityBody = GameManager.player.GetComponent<GravityBody> ();
		Vector3 objectiveUp = new Vector3(objective.transform.up.x,objective.transform.up.y,0f);
		transform.up = objectiveUp;
		
		Vector3 objectivePosition = new Vector3 (objective.transform.position.x, objective.transform.position.y, transform.position.z);
		objectivePosition += transform.up*1.2f;
		transform.position = objectivePosition;
	}

	//The objective has to be in the same planet as the player
	public void followObjective(GameObject objectiveGO){
		lastObjective = objective;
		objective = objectiveGO;
		timerChangingObjective = 0f;
		isChangingObjective = true;
	}

	public void followObjective(GameObject objectiveGO,float newAngleX,float newLerpMultiplyierUp,float newLerpMultiplyierPos,float newLerpMultiplyierZ){
		followObjective (objectiveGO);
		xAngle = newAngleX;
		lerpMultiplyierPos = newLerpMultiplyierPos;
		lerpMultiplyierUp = newLerpMultiplyierUp;
		lerpMultiplyierZPosition = newLerpMultiplyierZ;
	}

	public void followObjective(GameObject objectiveGO,float newAngleX,float newLerpMultiplyierPos){
		followObjective (objectiveGO,newAngleX,originalMultiplyierUp,newLerpMultiplyierPos,originalMultiplyierZ);
	}

	public void setNewXAngle(float newXAngle){
		xAngle = newXAngle;
	}

	public void resetXAngle(){
		xAngle = originalAngleX;
	}


	public void resetObjective(){
		followObjective (GameManager.player,originalAngleX,originalMultiplyierUp,originalMultiplyierPos,originalMultiplyierZ);
	}

	public void unfollowObjective(){
		objective = null;
		followingObjective = false;
	}
}



