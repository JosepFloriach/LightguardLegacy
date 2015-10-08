using UnityEngine;
using System.Collections;

public class IAControllerHydra : IAController {

	public GameObject headPosition;
	public GameObject hydraBody;
	public GameObject mesh;

	public GameObject otherHydra;

	public FrostFirePlanetEventsManager eventManager;

	public float minPlayerDistanceToAttack = 6f;
	public AttackType rangedAttack,meleeAttack,appearAttack;

	public GameObject platformDestroyed {get; set;}

	private bool isDoingMeleeAttack,isDoingRangedAttack,isDoingAppearAttack;
	private float meleeAttackTimer,rangedAttackTimer,appearAttackTimer;

	private enum ActualBehaviour{RangedAttack,MeleeAttack,Hide,Appear};
	private ActualBehaviour actualBehaviour;

	private bool isFinishedHidding = true;
	private bool isFinishedUnhidding = true;

	Collider collider;

	private float timerHidding = 0f;
	private float timerUnHidding = 0f;

	protected override void initialize(){
		collider = GetComponent<Collider> ();
		Attack rangedAttackA = attackController.getAttack(rangedAttack);
		rangedAttackA.informParent(gameObject);

		Attack meleeAttackA = attackController.getAttack(meleeAttack);
		meleeAttackA.informParent(gameObject);

		Attack appearAttackA = attackController.getAttack(appearAttack);
		appearAttackA.informParent(gameObject);
		
		actualBehaviour = ActualBehaviour.Appear;
		if(attackController.doAttack(appearAttack,false)){
			isDoingAppearAttack = true;
		}
	}

	protected override void UpdateAI(){

		if (!isFinishedUnhidding) {
			timerUnHidding+=Time.deltaTime;
			if(timerUnHidding>5f){
				isFinishedUnhidding = true;
				timerUnHidding = 0f;
			}
		}
		
		if (!isFinishedHidding) {
			timerHidding+=Time.deltaTime;
			if(collider.enabled && timerHidding>2f){
				enableHydraHitting(false);
			}
			if(timerHidding>5f){
				isFinishedHidding = true;
				timerHidding = 0f;
				if(platformDestroyed!=null){
					//platformDestroyed.GetComponent<HydraPlatform>().repositionPlatform();
				}

				GameObject closestPlatform = eventManager.getCloserHydraPlatformToPlayer(otherHydra);
				Vector3 position = closestPlatform.GetComponent<Renderer>().bounds.center;
				Vector3 direction = position - eventManager.transform.position;
				transform.position = position + (direction.normalized*1f);
			}
		}

		changeBehaviour ();
		doActualBehaviour ();
	}

	private void changeBehaviour(){
		if (!attackController.isDoingAnyAttack()) {
			//We check if we have to reset the melee and flying attack timers
			if(isDoingMeleeAttack){meleeAttackTimer = 0f; isDoingMeleeAttack =false;}
			if(isDoingRangedAttack){rangedAttackTimer = 0f; isDoingRangedAttack = false;}
			if(isDoingAppearAttack){appearAttackTimer = 0f; isDoingAppearAttack = false; isFinishedUnhidding = true;}
		}

		if (!attackController.isDoingAnyAttack() && isFinishedHidding && isFinishedUnhidding) {
			if(actualBehaviour.Equals(ActualBehaviour.Hide) && isFinishedHidding){
				actualBehaviour = ActualBehaviour.Appear;
			}else if(getPlayerDistance()>minPlayerDistanceToAttack){
				actualBehaviour = ActualBehaviour.Hide;
			}else{
				if(Random.value>0.5f){
					actualBehaviour = ActualBehaviour.MeleeAttack;
				}else{
					actualBehaviour = ActualBehaviour.RangedAttack;
				}
			}

		}
	}
	
	protected override bool virtualGetHurt ()
	{
		GameManager.audioManager.PlaySound (SoundIDs.E_GENERICHIT,AudioManager.STABLE,AudioManager.ENEMY);
		return base.virtualGetHurt ();
	}

	protected override void virtualDie ()
	{
		StartCoroutine (hydraDying ());
	}

	private IEnumerator hydraDying(){
		if(platformDestroyed!=null){
			platformDestroyed.GetComponent<HydraPlatform>().repositionPlatform();
		}
		yield return new WaitForSeconds (2f);
		GameManager.audioManager.PlaySound (SoundIDs.E_HYDRADIE,AudioManager.STABLE,AudioManager.ENEMY);
		
		/*if(platformDestroyed!=null){
			platformDestroyed.GetComponent<HydraPlatform>().repositionPlatform();
		}*/

	}

	public void enableHydraHitting(bool enable){
		collider.enabled = enable;
	}

	private void doActualBehaviour(){
		if(!isDead && !attackController.isDoingAnyAttack() && isFinishedHidding){
			if(actualBehaviour.Equals(ActualBehaviour.Hide)){
				iaAnimator.SetBool("isHidden",true);
				isFinishedHidding = false;
			}if(actualBehaviour.Equals(ActualBehaviour.Appear)){
				if(attackController.doAttack(appearAttack,false)){
					GameManager.audioManager.PlaySound (SoundIDs.E_HYDRAROAR,AudioManager.STABLE,AudioManager.ENEMY);
					
					isDoingAppearAttack = true;
				}
			}else if(actualBehaviour.Equals(ActualBehaviour.MeleeAttack)){
				GetComponent<CharacterController> ().LookLeftOrRight (getPlayerDirection());
				if(attackController.doAttack(meleeAttack,false)){
					GameManager.audioManager.PlaySound (SoundIDs.E_HYDRABITE,AudioManager.STABLE,AudioManager.ENEMY);
					
					isDoingMeleeAttack = true;
				}
			}else if(actualBehaviour.Equals(ActualBehaviour.RangedAttack)){
				GetComponent<CharacterController> ().LookLeftOrRight (getPlayerDirection());
				if(attackController.doAttack(rangedAttack,false)){
					GameManager.audioManager.PlaySound (SoundIDs.E_HYDRAFIRE,AudioManager.STABLE,AudioManager.ENEMY);
					
					isDoingRangedAttack = true;
				}
			}
		}
	}

}
