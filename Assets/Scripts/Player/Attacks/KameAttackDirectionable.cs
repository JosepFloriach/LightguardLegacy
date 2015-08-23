using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KameAttackDirectionable : Attack,AnimationSubscriber {

	
	//GameObjects
	public GameObject kameEffect;
	public GameObject kameCore;
	public GameObject elementalParticleSystem;
	public GameObject elementalParticleOnCharge;
	public GameObject enemyHitEffectPrefab;
	public GameObject directionalLine;
	
	//Public Variables
	public float totalTimeLasts = 0.9f;
	public float explosionTime = 0.6f;
	public float timeToDisappear = 0.2f;
	public float chargeTime = 0.6f;
	public float speed = 5f;
	public float forceExplosion = 10f;
	public float maxWidthKame = 0.2f;
	public float lineLength = 1f;
	public float maxTimeCharging = 1f;
	public bool explodes = true;
	public bool curved = true;
	public float slowTimeProportion = 0.1f;
	public int costPerSecond = 100;
	public float kameMinSize = 0.5f;
	public float kameMaxSize = 2f;
	public float extraScaleExplosion = 10f;

	//Private Variables
	private bool canDoNext = true;
	private float timer = 0f;
	private float timerCleanTrail = 0f;
	private Vector3 startPosition;
	private GameObject closestPlanet;
	private bool isCharged = false;
	private AnimationEventBroadcast eventHandler;
	private List<GameObject> enemiesHit;
	private int objectsCollided = 0;
	private Vector3 arrowDirection;
	private bool isCharging = false;
	private bool isDoingKame = false;
	private bool kameGoingRight = false;
	private float chargeTimer = 0f;
	private bool started = false;
	private bool hasHitGround = false;
	private Vector3 originalScale;
	private int pointsSpent = 0;
	private float scaleMultiplyier;
	private Vector3 explosionScale;
	private bool detonate = false;

	//Variables that need to be initialized at the beginning
	public override void initialize(){
		explosionScale =  new Vector3(extraScaleExplosion,extraScaleExplosion,extraScaleExplosion);
		originalScale = kameEffect.transform.localScale;
		attackType = AttackType.KameDirectional;
		eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
		Color trailsColor = kameCore.GetComponent<ParticleSystemRenderer> ().material.GetColor("_TintColor");
		TrailRenderer tr = kameEffect.GetComponent<TrailRenderer>();
		TrailRenderer[] renderers = kameEffect.GetComponentsInChildren<TrailRenderer> ();
		for(int i = 0;i<renderers.Length;++i){
			renderers[i].material.color = trailsColor;
		}
		tr.material.color = trailsColor;
	}

	//When it hits an enemy
	public override void enemyCollisionEnter(GameObject enemy,Vector3 point){
		if(!enemiesHit.Contains(enemy) && !enemy.GetComponent<IAController>().isDead){
			enemiesHit.Add(enemy);
			//If it's an enemy we damage him
			enemy.GetComponent<IAController>().getHurt(damage,point,true);
			
			//We find the radius of areaEffect
			//enemy.GetComponent<Rigidbody>().AddExplosionForce(forceExplosion,transform.position,1f);
			GameObject newEffect = GameObject.Instantiate (enemyHitEffectPrefab) as GameObject;
			newEffect.transform.position = enemy.GetComponent<Rigidbody> ().worldCenterOfMass - (kameEffect.transform.forward * 0.15f);
			Vector3 direction = (enemy.transform.position - kameEffect.transform.position).normalized + (enemy.transform.up * 1f);
			//enemy.GetComponent<Rigidbody> ().AddForce (direction.normalized * forceExplosion,ForceMode.Impulse);
			//enemy.GetComponent<Rigidbody>().velocity += direction * forceExplosion;
			/*if(detonate || (hasHitGround && explodes)){
				enemy.GetComponent<IAController>().sendFlying(direction.normalized*forceExplosion);
			}else{
				enemy.GetComponent<IAController>().hitInterruptsAndHitstone();
				enemy.GetComponent<Rigidbody> ().AddForce (direction.normalized * forceExplosion,ForceMode.Impulse);

			}*/
			enemy.GetComponent<IAController>().sendFlyingByForce(direction.normalized*forceExplosion * scaleMultiplyier);
			
			GameManager.comboManager.addCombo ();
			if(!elementAttack.Equals(ElementType.None)){
				AttackElementsManager.getElement(elementAttack).doEffect(enemy);
			}
		}
	}

	//When the center of the ball hits the ground
	public void centerHit(GameObject other){
		if(other.layer.Equals(LayerMask.NameToLayer("Planets"))){
			hasHitGround = true;
		}
	}

	/**********************************  START OF THE KAME PIPELINE   *************************************/

	//1. Called when the button is pressed, it starts the kame animation
	public override void startAttack(){
		if(!isCharging && !isDoingKame && !started){
			started = true;
			GameManager.audioManager.PlayStableSound(11);
			GameManager.playerAnimator.SetBool ("isChargingDirectionalKame",true);
		}
	}


	//2. Initializes the kame and shows the direction line (Called when the animation starts)
	private void animationStarted(){
		chargeTimer = 0f;
		kameEffect.transform.rotation = Quaternion.LookRotation (GameManager.player.transform.forward, GameManager.player.transform.up);
		//Find closest planet
		GameObject[] planets = GravityAttractorsManager.getGravityAttractors ();
		float smallestDistance = float.PositiveInfinity;
		foreach(GameObject planet in planets){
			if(Vector3.Distance(planet.transform.position,GameManager.player.transform.position)<smallestDistance){
				closestPlanet = planet;
				smallestDistance = Vector3.Distance(planet.transform.position,GameManager.player.transform.position);
			}
		}
		//Initialize elements
		hasHitGround = false;
		isFinished = false; 
		kameEffect.SetActive (false);
		kameCore.SetActive (true);
		kameCore.GetComponent<ParticleSystem>().Play();
		startPosition = ((GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass + (GameManager.player.transform.up * 0.15f))) + (kameEffect.transform.forward * 0.4f);
		kameEffect.transform.position = startPosition;
		kameCore.transform.position = startPosition;
		kameCore.transform.forward = kameEffect.transform.forward;
		objectsCollided = 0;
		enemiesHit = new List<GameObject> (0);
		transform.parent = closestPlanet.transform;
		if(!elementAttack.Equals(ElementType.None)){
			elementalParticleOnCharge.SetActive(true);
			elementalParticleOnCharge.GetComponent<ParticleSystem>().Play ();
			Material material = AttackElementsManager.getElement(elementAttack).material;
			if(material!=null){
				elementalParticleSystem.GetComponent<ParticleSystemRenderer >().material = material;
				elementalParticleOnCharge.GetComponent<ParticleSystemRenderer >().material = material;
			}
		}
		//Starts to update the charging up of the kame
		isCharged = false;
		isCharging = true;
		Util.changeTime (slowTimeProportion);
		isFinished = false;
		arrowDirection = GameManager.player.transform.forward;
		directionalLine.SetActive (true);
		directionalLine.GetComponent<LineRenderer> ().SetPosition (0, GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass);
		directionalLine.GetComponent<LineRenderer> ().SetPosition (1, GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass + (lineLength * arrowDirection.normalized));
		StartCoroutine (updateCharge ());
	}

	//3. Updates the charging of the kame (It also limits the time that it can be charging)
	private IEnumerator updateCharge(){
		float pointsAccumulated = 0f;
		pointsSpent = 0;
		while(isCharging || isDoingKame){
			pointsAccumulated+=(Time.deltaTime/slowTimeProportion) * costPerSecond;
			timer+=Time.deltaTime;
			//float ratio = (1f - ((timer) / (chargeTime))) * startChargeScale;
			kameCore.transform.localScale = new Vector3 (0f, 0f, 0f);
			kameCore.transform.position = GameManager.playerController.lightGemObject.transform.position;
			elementalParticleOnCharge.transform.position = GameManager.playerController.lightGemObject.transform.position;
			
			kameEffect.transform.position = GameManager.playerController.lightGemObject.transform.position;
			if(!isDoingKame){
				if(pointsAccumulated>1){
					if(!GameManager.lightGemEnergyManager.canDoSpecialAttack((int)pointsAccumulated)){
						GameManager.lightGemEnergyManager.substractPoints((int)pointsAccumulated);
						pointsSpent+=(int)pointsAccumulated;
						pointsAccumulated = 0;
						buttonReleased();
					}else{
						GameManager.lightGemEnergyManager.substractPoints((int)pointsAccumulated);
						pointsSpent+=(int)pointsAccumulated;
						pointsAccumulated = 0;
					}
				}
			}
			elementalParticleSystem.transform.position = kameEffect.transform.position;
			if(isDoingKame){
				GameManager.playerSpaceBody.setHasToApplyForce(false);
				GameManager.player.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
			}
			directionalLine.GetComponent<LineRenderer> ().SetPosition (0, GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass);
			directionalLine.GetComponent<LineRenderer> ().SetPosition (1, GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass + (lineLength * arrowDirection.normalized));
			yield return null;
		}
		GameManager.playerSpaceBody.setHasToApplyForce(true);
	}

	//4. Sets the arrow direction depending on the input (Called while charging the kame)
	public override void receiveInputDirections(float inputVertical,float inputHorizontal){
		chargeTimer += Time.deltaTime;
		if(chargeTimer>maxTimeCharging){
			buttonReleased();
		}else{
			if(Mathf.Abs(inputVertical)>0.05f || Mathf.Abs(inputHorizontal)>0.05f){
				Vector3 newArrow =  new Vector3 (inputHorizontal,inputVertical,0f);
				float angle = Util.getAngleFromVectorAToB (Vector3.up,GameManager.player.transform.up);
				arrowDirection =  Quaternion.Euler (new Vector3 (0f,0f,-angle)) * newArrow;
				GameManager.player.GetComponent<CharacterController>().LookLeftOrRight(inputHorizontal);
			}
		}
	}

	//5. Called when the button is released, it starts the release of the kame and stops the charge
	public override void buttonReleased(){
		if(!isDoingKame && isCharging){
			kameGoingRight = GameManager.playerController.getIsLookingRight();
			isDoingKame = true;
			GameManager.playerAnimator.SetBool ("isChargingDirectionalKame",false);
			if(canDoNext && isCharging){
				isCharging = false;
				Util.changeTime (1f);
				
			}
			directionalLine.SetActive (false);
		}
	}

	//6. It moves the kame "forward" and makes it explode at the end
	IEnumerator doKame(){
		float ratioSizeKame = (float)pointsSpent / (float)costPerSecond;
		scaleMultiplyier = ((kameMaxSize - kameMinSize) * ratioSizeKame)+kameMinSize;
		kameEffect.transform.localScale = originalScale * scaleMultiplyier;
		//Throws the Kame
		enemiesHit = new List<GameObject> (0);
		isCharged = true;
		isFinished = true;
		kameEffect.SetActive (true);
		
		//THROW THE KAME START
		GameManager.audioManager.PlayStableSound(2);
		kameEffect.transform.position = GameManager.playerController.lightGemObject.transform.position - (kameEffect.transform.forward.normalized * speed * Time.deltaTime * 2f);
		kameCore.GetComponent<ParticleSystem>().Stop();
		if(!elementAttack.Equals(ElementType.None)){
			elementalParticleSystem.SetActive(true);
			elementalParticleSystem.GetComponent<ParticleSystem>().Play();
		}
		//Cleans the trails before throwing the kame
		StartCoroutine ("resetTrails");
		timer = 0f;
		kameEffect.transform.forward = arrowDirection;
		
		detonate = false;
		//Kame flying
		while(timer<totalTimeLasts){
			if (detonate || (hasHitGround && explodes)) {break;}
			timer+=Time.deltaTime;
			float ratio = (timer)/totalTimeLasts;
			
			//Calculate the kame's new position
			Vector3 newPostion = kameEffect.transform.position + (kameEffect.transform.forward.normalized * speed * Time.deltaTime) ;
			
			kameEffect.transform.position = newPostion;
			
			elementalParticleSystem.transform.position = kameEffect.transform.position;
			Vector3 planetDirection = kameEffect.transform.position - closestPlanet.transform.position;
			planetDirection.z = 0f;
			float planetRadius = closestPlanet.GetComponent<GravityAttractor>().getSphereRadius();
			if(curved){
				arrowDirection = ((arrowDirection.normalized)-(planetDirection.normalized * Time.deltaTime /planetRadius * speed)).normalized;
				kameEffect.transform.forward = arrowDirection;
			}
			yield return null;
		}
		elementalParticleOnCharge.SetActive(false);
		//THROW THE KAME END
		kameCore.GetComponent<ParticleSystem>().Stop();
		kameEffect.GetComponent<ParticleSystem>().Stop();
		timer = 0;
		//Kame Explosion
		enemiesHit.Clear ();
		while (timer < explosionTime) {
			timer+=Time.deltaTime;
			kameEffect.transform.localScale += (Time.deltaTime * explosionScale * scaleMultiplyier);
			Color color = kameEffect.GetComponent<Renderer>().material.GetColor("_TintColor");
			color.a =  1f-(timer/explosionTime);
			kameEffect.GetComponent<Renderer>().material.SetColor("_TintColor",color);
			yield return null;
		}
		//End explosion
		
		//CLEAN THE KAME START
		started = false;
		
		if(!elementAttack.Equals(ElementType.None)){
			elementalParticleSystem.GetComponent<ParticleSystem>().Stop();
			elementalParticleSystem.SetActive(true);
		}
		kameCore.SetActive (false);
		kameEffect.SetActive (false);
		yield return new WaitForSeconds (timeToDisappear);
		canDoNext = true;
		//CLEAN THE KAME END
	}
	
	//Method to be called when the kame has been thrown and it makes it explode
	public void Detonate() {
		detonate = true;
	}


	//Methods to clean the kame trail
	private IEnumerator resetTrails(){
		TrailRenderer tr = kameEffect.GetComponent<TrailRenderer>();
		TrailRenderer[] renderers = kameEffect.GetComponentsInChildren<TrailRenderer> ();
		float[] tempoTimes = new float[renderers.Length];
		float tmp = tr.time;
		tr.time = -1;
		for(int i = 0;i<renderers.Length;++i){
			tempoTimes[i] = renderers[i].time;
			renderers[i].time = -1;
		}
		yield return new WaitForEndOfFrame();
		
		for(int i = 0;i<renderers.Length;++i){
			renderers[i].time = tempoTimes[i];
		}
		tr.time = tmp;
		kameEffect.GetComponent<TrailRenderer>().startWidth = maxWidthKame;
		kameEffect.GetComponent<TrailRenderer>().endWidth = 0f ;
		yield return null;
	}
	//Methods to clean the kame trail
	private IEnumerator cleanKameTrail(){
		timerCleanTrail = 0f;
		while(timerCleanTrail<timeToDisappear){
			timerCleanTrail+=Time.deltaTime;
			float ratio = timerCleanTrail/timeToDisappear;
			float invertRatio = 1f - ratio;
			kameEffect.GetComponent<TrailRenderer>().startWidth = maxWidthKame * invertRatio;
			kameEffect.GetComponent<TrailRenderer>().endWidth = maxWidthKame * invertRatio;
			yield return null;
		}
	}


	//OTHER METHODS
	public bool getIsCharging(){
		return isCharging;
	}

	public override bool canReceiveInputDirections(){
		return isCharging;
	}

	public override bool lockMovement(){
		return isCharging || isDoingKame;
	}

	public override void interruptAttack(){
		if(isCharging){
			cancelCharge ();
		}
	}

	//Cancels the charging of the kame (If interrupted)
	public void cancelCharge(){
		if (isCharging) {
			buttonReleased();
		}
	}
	
	public override bool canDoNextAttack(){
		return canDoNext;
	}

	public bool getIsDoingKame(){
		return isDoingKame;
	}
	public override bool isAttackFinished(){
		return isFinished || canDoNext;
	}
	
	void AnimationSubscriber.handleEvent(string idEvent) {
		switch (idEvent) {
		case "start": 
			break;
		case "startCharge": 
			if(started){
				animationStarted();
			}
			break;
		case "end": 
			isDoingKame = false;
			StartCoroutine("doKame");
			break;
		default: 
			break;
		}
	}
	
	
	string AnimationSubscriber.subscriberName() {
		return  "KameDirectional";	
	}
}
