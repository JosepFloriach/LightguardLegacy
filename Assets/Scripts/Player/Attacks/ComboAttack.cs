using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboAttack : Attack, AnimationSubscriber {

	private AnimationEventBroadcast eventHandler;
	private Collider stick;
	private Collider leg;
	private Collider fist;

	private AttackCollider attackColliderStick;
	private AttackCollider attackColliderLeg;
	private AttackCollider attackColliderFist;
	private Xft.XWeaponTrail weaponEffects;
	private float timeOfAnimation;
	
	private List<GameObject> enemiesHit;
	private bool hasHitEnemy;
	private bool isComboing;
	private SlashController slashController;
	private string[] correspondancesComboBodyPart = {"Weapon","Leg","Weapon","Weapon","Weapon","Fist"};
	private bool lastEnded = true;
	char[] comboNumberSeparator = { '-' };


	public override void initialize() {
		attackType = AttackType.Combo;
		eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
		
		stick = GameManager.player.GetComponent<PlayerController>().weapon.GetComponentInChildren<Collider>();
		leg = GameManager.player.GetComponent<PlayerController> ().playerLegObject.GetComponent<Collider> ();
		fist = GameManager.player.GetComponent<PlayerController> ().playerFistObject.GetComponent<Collider> ();

		attackColliderStick = GameManager.player.GetComponent<PlayerController>().weapon.GetComponentInChildren<AttackCollider>();
		attackColliderLeg = GameManager.player.GetComponent<PlayerController> ().playerLegObject.GetComponent<AttackCollider> ();
		attackColliderFist = GameManager.player.GetComponent<PlayerController> ().playerFistObject.GetComponent<AttackCollider> ();
		weaponEffects = GameManager.player.GetComponent<PlayerController>().weapon.GetComponentInChildren<Xft.XWeaponTrail>();
		slashController = GameManager.player.GetComponent<PlayerController>().weapon.GetComponentInChildren<SlashController>();
		enemiesHit = new List<GameObject> (0);
		//weaponEffects.StopSmoothly(0.1f);
	}

	//Cuando se hace el combo 3 con el enemies hit  activo, los enemigos no son afectados por el tercer golpe. 
	public override void enemyCollisionEnter(GameObject enemy,Vector3 point) {
		if(!enemiesHit.Contains(enemy) && !enemy.GetComponent<IAController>().isDead){
			enemiesHit.Add(enemy);
			if(correspondancesComboBodyPart [combosteep - 1].Equals ("Weapon")){
				enemy.GetComponent<IAController>().getHurt(1,point,false);
			}else{
				enemy.GetComponent<IAController>().getHurt(1,point,true);
			}

			enemy.GetComponent<IAController>().hitCanSendFlying();
			GameManager.comboManager.addCombo ();
			
			//Pruebas de que lanze al enemigo por los aires)
			//Vector3 direction = enemy.GetComponent<Rigidbody>().worldCenterOfMass - GameManager.player.transform.position;
			//enemy.GetComponent<Rigidbody>().velocity+=(direction*4f);
			if(!hasHitEnemy){
				hasHitEnemy = true;
				GameManager.lightGemEnergyManager.addPoints(1);
			}
		}
	}

	private Collider getCorrespondingCollider(){
		if (combosteep > 0 && combosteep <= correspondancesComboBodyPart.Length) {
			if (correspondancesComboBodyPart [combosteep - 1].Equals ("Weapon")) {
				return stick;
			} else if (correspondancesComboBodyPart [combosteep - 1].Equals ("Leg")) {
				return leg;
			} else if (correspondancesComboBodyPart [combosteep - 1].Equals ("Fist")) {
				return fist;
			}
		}
		return null;
	}

	private void setAttackColliders(){
		attackColliderStick.attack = gameObject;
		attackColliderLeg.attack = gameObject;
		attackColliderFist.attack = gameObject;
	}

	public bool isComboable = false;
	public int combosteep = 0;
	
	
	public override void startAttack(){	
		if (isComboable) {
			isComboing = true;
		}
		if (!isComboing) {
				//stick.enabled = true;
				//weaponEffects.Activate();
				isFinished = false;
				GameManager.playerAnimator.SetBool("doComboAttack",true);
				//GameManager.playerAnimator.SetTrigger("doComboAttack");	
				combosteep = 0;
				lastEnded = true;
		} else if (isComboable) {
				isFinished = false;
				//GameManager.playerAnimator.SetTrigger("doComboAttack");
		}
	}

	public void enableHitbox() {
		if(!isFinished){
			GameManager.audioManager.PlaySound(3);
			if(getCorrespondingCollider()!=null){
				getCorrespondingCollider().enabled = true;
			}
			setAttackColliders();
			//stick.enabled = true;
			enemiesHit = new List<GameObject>(0);
			hasHitEnemy = false;
		}
	}
	
	public void allowChaining() {
		if (lastEnded) {
			lastEnded = false;
		}
		isComboing = false;
		isComboable = true;
	}
	
	public void disableHitbox() {
		if(getCorrespondingCollider()!=null){
			getCorrespondingCollider().enabled = false;
		}
		//getCorrespondingAttackCollider ().attack = null;
	}
	
	public void endCombo() {
		//gameObject.GetComponent<ParticleSystem>().enableEmission = true;
		weaponEffects.StopSmoothly (0.5f);
		//eventHandler.unsubscribe(this);
		isComboable = false;
		if (!isComboing) {
			isFinished = true;
			GameManager.playerAnimator.SetBool ("doComboAttack", false);
			lastEnded = false;
		} else {
			isComboing = false;
			GameManager.playerAnimator.SetBool("doComboAttack",true);
			lastEnded = true;
		}

		if (getCorrespondingCollider () != null) {
			getCorrespondingCollider ().enabled = false;
		}

		//attackCollider.attack = null;

		//print ("combo ended");
	}
	
	public override bool isAttackFinished(){
		return isFinished;
	}

	public override bool canDoNextAttack()
	{
		return isFinished || isComboable;
	}

	public override void interruptAttack(){
		endCombo ();
	}


	
	void AnimationSubscriber.handleEvent(string idEvent) {
		//If it contains the combo we check which step of the combo is it
		if(idEvent.Contains("comboStart")){
			//Debug.Log("Combo "+Time.time);
			
			combosteep = int.Parse(idEvent.Split((char[])comboNumberSeparator)[1]);
			allowChaining();
			enableHitbox();
		}else if(idEvent.Contains("combo")){
			//Debug.Log("Combo "+Time.time);

			combosteep = int.Parse(idEvent.Split((char[])comboNumberSeparator)[1]);
			allowChaining();
		}else{
			switch (idEvent) {
			case "start": 
				//Debug.Log("start "+Time.time);
				enableHitbox();
				break;
			case "end":
				//Debug.Log("end "+Time.time);
				disableHitbox();
				break;
			case "done":
				//Debug.Log("done "+Time.time);
				endCombo();
				break;
			case "doSlash":
				//Debug.Log("done "+Time.time);
				slashController.doSlash();
				break;
			default: 
			break;
			}
		}
	}
	
	string AnimationSubscriber.subscriberName() {
		return  "ComboAttack";	
	}
	
	
}
