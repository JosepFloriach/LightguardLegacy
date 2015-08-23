using UnityEngine;
using System.Collections;

public class MundusFisureAttack : Attack {

	public GameObject fisurePrefab;
	public float timeToChargeAttack = 1f;
	public float timeToGrow = 1f;
	public float maxScaleSphere = 5f;
	public float timeGoingUp = 3f;
	public float explodingTime = 4f;
	public float scaleOnExplosion = 70f;
	public float cameraZPositionTotal = 50f;
	//Private variables
	private GameObject parent;
	private Animator iaAnimator;
	private OutlineChanging outlineChanger;
	private bool interrupted = false;
	private bool hasHurtPlayer = false;
	private GameObject surroundingBall;
	private bool hasBallTouchedFloor = false;
	private Vector3 pointCollided;
	private bool isExploding;
	private bool hasHitPlayer;
	
	public override void initialize(){
		attackType = AttackType.MundusFisureAttack;
	}
	
	public override void otherCollisionEnter(GameObject enemy,Vector3 point){
		if(enemy.layer.Equals(LayerMask.NameToLayer("Planets"))){
			hasBallTouchedFloor = true;
			pointCollided = point;
		}else if(enemy.layer.Equals(LayerMask.NameToLayer("Player")) && isExploding && !hasHitPlayer){
			GameManager.playerController.getHurt(damage,point);
		}
	}
	
	IEnumerator doAttack(){
		surroundingBall = parent.GetComponent<IAControllerMundus> ().useNewSurroundingBall ();
		if(surroundingBall!=null){
			hasBallTouchedFloor = false;
			isExploding = false;
			hasHitPlayer = false;

			surroundingBall.GetComponent<Collider>().enabled = true;
			surroundingBall.GetComponent<AttackCollider>().attack = gameObject;
			float timer = 0f;
			parent.GetComponent<GravityBody> ().setHasToApplyForce (false);
			parent.GetComponent<Rigidbody> ().isKinematic = true;

			while(timer<timeGoingUp){
				timer+=Time.deltaTime;
				parent.transform.position+= Time.deltaTime * parent.transform.up;
				yield return null;
			}

			float originalZLerp = GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().lerpMultiplyierZPosition;
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().lerpMultiplyierZPosition = 0.05f;
			float zPosition = GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().getObjectiveZ();
			parent.GetComponent<IAControllerMundus> ().setProtecting();

			surroundingBall.GetComponent<SplineWalkerMultipurpose> ().enabled = false;
			Vector3 ballOriginalPosition = surroundingBall.transform.position;
			Vector3 objectivePosition = parent.GetComponent<Rigidbody> ().worldCenterOfMass + new Vector3(0f,0f,-2f);
			iaAnimator.SetBool ("isChargingFisureAttack", true);
			timer = 0f;
			while(timer<timeToChargeAttack){
				timer+=Time.deltaTime;
				float ratio = timer/timeToChargeAttack;
				surroundingBall.transform.position = Vector3.Lerp(ballOriginalPosition,objectivePosition,ratio);
				yield return null;
			}
			Vector3 originalScale = surroundingBall.transform.localScale;
			Vector3 objectiveScale = new Vector3(maxScaleSphere,maxScaleSphere,maxScaleSphere);
			timer = 0f;
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setObjectiveZ(cameraZPositionTotal);
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setCameraShaking();
			while(timer<timeToGrow){
				timer+=Time.deltaTime;
				float ratio = timer/timeToGrow;
				Vector3 scale = Vector3.Lerp(originalScale,objectiveScale,ratio);
				surroundingBall.transform.localScale = scale;
				yield return null;
			}

			iaAnimator.SetBool ("isChargingFisureAttack", false);

			timer = 0f;
			while(!hasBallTouchedFloor){
				timer+=Time.deltaTime;
				surroundingBall.transform.position-= Time.deltaTime * 2f * parent.transform.up;
				yield return null;
			}

			yield return new WaitForSeconds(0.2f);

			GameObject fisureInstance = GameObject.Instantiate(fisurePrefab) as GameObject;
			pointCollided.z = parent.transform.position.z;
			fisureInstance.transform.position = pointCollided - parent.transform.up;
			fisureInstance.transform.up = parent.transform.up;
			parent.GetComponent<IAControllerMundus>().informFisure(fisureInstance);

			isExploding = true;
			timer = 0f;
			Vector3 objectiveScaleExplosion = new Vector3(scaleOnExplosion,scaleOnExplosion,scaleOnExplosion);
			while(timer<explodingTime){
				timer+=Time.deltaTime;
				float ratio = timer/explodingTime;
				Vector3 scale = Vector3.Lerp(objectiveScale,objectiveScaleExplosion,ratio);
				surroundingBall.transform.localScale = scale;
				changeColor(1f - ratio);
				yield return null;
			}

			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().lerpMultiplyierZPosition = originalZLerp;
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().returnOriginalZ();

			surroundingBall.SetActive(false);

			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().stopCameraShaking();

			parent.GetComponent<IAControllerMundus> ().stopProtecting();
			parent.GetComponent<GravityBody> ().setHasToApplyForce (true);
			parent.GetComponent<Rigidbody> ().isKinematic = false;
		}
		isFinished = true;
	}

	private void changeColor(float newAlpha){
		foreach(Renderer r in surroundingBall.GetComponentsInChildren<Renderer>()){
			Color color = r.GetComponent<Renderer>().material.GetColor("_TintColor");
			color.a =  newAlpha/4f;
			r.GetComponent<Renderer>().material.SetColor("_TintColor",color);
		}
	}
	
	public override void startAttack(){
		if(isFinished){
			StartCoroutine("doAttack");
			isFinished = false;
			interrupted = false;
		}
	}

	public override void setTier(int tier){
		
	}
	
	public override void informParent(GameObject parentObject){
		transform.parent = parentObject.transform;
		transform.rotation = parentObject.transform.rotation;
		transform.position = parentObject.GetComponent<Rigidbody>().worldCenterOfMass + (parentObject.transform.forward*parentObject.GetComponent<WalkOnMultiplePaths>().centerToExtremesDistance*1.3f);
		parent = parentObject;
		iaAnimator = parent.GetComponent<IAController> ().getIAAnimator ();
		outlineChanger = parent.GetComponent<OutlineChanging> ();
	}
}
