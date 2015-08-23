using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (OutlineChanging))]
public class PlayerController : MonoBehaviour {

	public float moveSpeed = 5f;
	public float normalJumpForce = 15f;
	public float spaceJumpForce = 100f;
	public GameObject particleSystemJumpCharge;
	public GameObject animationBigPappada;
	public GameObject getHurtBigPappadaPrefab;
	//public GameObject breathingBubble;
	public GameObject lightGemObject;
	public GameObject playerTongueObject;
	public GameObject playerNeckObject;
	public GameObject playerLegObject;
	public GameObject playerFistObject;
	public GameObject playerTongueColliderObject;
	public GameObject particlesOnDematerialize;
	public GameObject particlesOnMaterialize;
	public float timeBetweenDamageWhenNotBreathing = 0.5f;
	public int damageWhenNotBreathing = 1;
	//public float minimumBreathingBubbleScale = 6f;
	//public float maximumBreathingBubbleScale = 1f;


	public float timeToDieInSpace = 4f;

	public float centerToExtremesDistance = 0f;
	public float extraSafeDistanceFromEnemies = 0.3f;
	public bool isInvulnerable = false;
	public bool isInsidePlanet = false;

	public GameObject pappada;

	//SpaceJump line
	public float lineJumpDistance;
	private LineRenderer lineRenderer;
	private bool isShowingLineJump;
	private Vector3 lineJumpDirection;
	public GameObject flyingParticles;
	private ParticleSystem flyParticles;
	private bool isFinishedSpaceJump;
	public GameObject explosionOnDieInSpacePrefab;
	public GameObject weapon;
	public float invulnerableTimeOnFallDown = 1f;
	public float invulnerableTimeAfterFallDown = 1f;

	private CharacterController characterController;
	private CharacterAttackController attackController;
	private Animator bpAnimator;
	private SpaceGravityBody body;
	private Vector3 smoothMoveVelocity;
	private bool isSpaceJumping = false;
	private bool isChargingSpaceJump = false;
	private bool gotHit = false;
	private bool isOutsideAthmosphere;
	private float timeHasBeenInSpace = 0f;
	private Killable killable;
	private float timeHasNotBeenBreathing;
	private PappadaController pappadaC;
	private bool canDrownInSpace = true;
	private bool isFallingDown = false;
	public GameObject getHurtBigPappada;

	private bool isDematerialized = false;
	void Awake(){
		GameManager.registerPlayer (gameObject);	
	}

	void Start () {
		body = GetComponent<SpaceGravityBody> ();
		killable = GetComponent<Killable> ();
		characterController = GetComponent<CharacterController> ();
		attackController = GetComponent<CharacterAttackController> ();
		GameObject attack = GameObject.Find("skillAttack");
		initializePlayerRotation ();
		bpAnimator = animationBigPappada.GetComponent<Animator>();
		pappadaC = pappada.GetComponent<PappadaController> ();
		flyParticles = flyingParticles.GetComponent<ParticleSystem> ();
		getHurtBigPappada = GameObject.Instantiate (getHurtBigPappadaPrefab) as GameObject; 
		initializeVariables ();
		StartCoroutine ("resetWeaponTrail");
	}

	public void initializePlayerRotation(){
		characterController.setOriginalOrientation ();
	}

	IEnumerator resetWeaponTrail(){
		Xft.XWeaponTrail weaponTrail = weapon.GetComponentInChildren<Xft.XWeaponTrail> ();
		//weapon.GetComponentInChildren<Xft.XWeaponTrail> ().Deactivate ();
		//We reset the weapon trail because otherwise it will come out of big P original position in space
		Color color = weaponTrail.MyColor;
		weaponTrail.MyColor = new Color (0f, 0f, 0f, 0f);
		yield return new WaitForSeconds(1f);
		weaponTrail.MyColor = color;
		weaponTrail.Deactivate ();
	}

	private void initializeVariables(){
		characterController.stopJumping ();
		isSpaceJumping = false;
		timeHasNotBeenBreathing = timeBetweenDamageWhenNotBreathing;
		timeHasBeenInSpace = 0f;
		centerToExtremesDistance = (animationBigPappada.GetComponent<Collider>().bounds.size.z /2f)+extraSafeDistanceFromEnemies;
		isInvulnerable = false;
		GetComponent<Rigidbody>().velocity = new Vector3 (0f, 0f, 0f);

		lineRenderer = GetComponent<LineRenderer> ();

		//Initialize the animator
		if(bpAnimator!=null){
			bpAnimator.SetBool("isJumping",false);
			bpAnimator.SetBool("isSpaceJumping",false);
			bpAnimator.SetBool("isGoingUp",false);
			bpAnimator.SetBool("isChargingSpaceJumping",false);
			bpAnimator.SetBool("isWalking",false);
			bpAnimator.SetBool("isDerribado",false);
		}

		isSpaceJumping = false;
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().returnOriginalZ();
		HideArrow();
		isFinishedSpaceJump = true;
		flyParticles.Stop();
		body.setIsGettingOutOfOrbit (false);
		canDrownInSpace = true;

		if(isDematerialized){
			StartCoroutine(rematerialize());
		}
	}

	void Update() {
		if(characterController.getIsJumping()){
			if (characterController.getIsGoingUp()) {
				bpAnimator.SetBool("isGoingUp",true);
			}else{
				bpAnimator.SetBool("isGoingUp",false);
			}
		}else{
			FinishJump();
			if(isSpaceJumping){
				FinishSpaceJump();
			}
		}

		bool canBreatheInActualPlanet = GameManager.playerSpaceBody.canBreatheInActualPlanet ();
		if(body.getUsesSpaceGravity() || (!canBreatheInActualPlanet)){
			if(!body.getIsOutsideAthmosphere() && canBreatheInActualPlanet){
				timeHasBeenInSpace = 0f;
			}
			float ratio = 1f - (timeHasBeenInSpace/timeToDieInSpace);
			GUIManager.setPercentageOfBreathing(ratio);
		}

		
		if((body.getIsOutsideAthmosphere() || !canBreatheInActualPlanet) && canDrownInSpace){

			//breathingBubble.SetActive(true);
			GUIManager.activateSpaceJumpGUI();
			//rigidbody.velocity = rigidbody.velocity.normalized * (Constants.GRAVITY_FORCE_OF_PLANETS/1.5f);

			if(!GameManager.isGameEnded){
				if(timeHasBeenInSpace>=timeToDieInSpace){
					//breathingBubble.transform.localScale = new Vector3(0f,0f,0f);
					timeHasNotBeenBreathing+=Time.deltaTime;
					if(timeHasNotBeenBreathing>=timeBetweenDamageWhenNotBreathing){
						dieInSpace();
					}
				}else{
					timeHasBeenInSpace += Time.deltaTime;
					float ratio = 1f - (timeHasBeenInSpace/timeToDieInSpace);
					//float newScale = ((maximumBreathingBubbleScale - minimumBreathingBubbleScale) * ratio)+minimumBreathingBubbleScale;
					//breathingBubble.transform.localScale = new Vector3(newScale,newScale,newScale);
				}
			}

		}else{
			if(!GameManager.isGameEnded && canBreatheInActualPlanet){
				timeHasBeenInSpace = 0f;
				//breathingBubble.SetActive(false);
			}
		}

		if(!isShowingLineJump){
			HideArrow();
		}else{
			ActArrow();
		}
	}

	public void resetBreathing(){
		timeHasBeenInSpace = 0f;
	}

	void FinishSpaceJump(){
		body.setIsGettingOutOfOrbit (false);
		GUIManager.activatePlayingGUIWithFadeIn ();
		bpAnimator.SetBool("isSpaceJumping",false);
		isSpaceJumping = false;
		flyParticles.Stop();
		if(GameManager.playerSpaceBody.getClosestPlanet().GetComponent<Planet>().centerCameraOnLand){
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().returnOriginalZ();
		}else{
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setObjectiveZCameraSmallPlanet();
		}
		HideArrow();
		isFinishedSpaceJump = true;
	}

	void FinishJump(){
		bpAnimator.SetBool("isJumping",false);
		characterController.stopJumping ();
	}

	public void dieInSpace(){
		if(!killable.isDead()){
			kill ();
			flyParticles.Stop();
			GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
			GameObject newEffect = GameObject.Instantiate(explosionOnDieInSpacePrefab) as GameObject;
			newEffect.transform.position = transform.position;
			timeHasNotBeenBreathing = 0f;
		}
	}

	public void SpaceJump() {
		GUIManager.activateSpaceJumpGUI();
		characterController.Jump (spaceJumpForce);
		GetComponent<Rigidbody> ().velocity = lineJumpDirection * spaceJumpForce;
		//If we jump into the space, stop the particle system.
		ParticleSystem particles = particleSystemJumpCharge.GetComponent<ParticleSystem> ();
		particles.Stop ();
		bpAnimator.SetBool("isSpaceJumping",true);
		bpAnimator.SetBool("isChargingSpaceJumping",false);
		isChargingSpaceJump = false;
		isSpaceJumping = true;
		HideArrow ();
		flyParticles.Clear();
		flyParticles.Play();
		isFinishedSpaceJump = false;
		body.setIsGettingOutOfOrbit (true);

		GUIManager.deactivateCorruptionBar ();
	}

	public void SpaceJump(Vector3 direction,bool isGettingOut){
		lineJumpDirection = direction;
		SpaceJump ();
		body.setIsGettingOutOfOrbit (isGettingOut);
	}

	public void Jump() {
		GameManager.audioManager.PlaySound(4);
		ParticleSystem particles = particleSystemJumpCharge.GetComponent<ParticleSystem> ();
		particles.Stop ();
		bpAnimator.SetBool("isChargingSpaceJumping",false);
		isChargingSpaceJump = false;
		bpAnimator.SetBool("isJumping",true);
		characterController.Jump (normalJumpForce);
		body.applySpaceBodyChangesOnJump ();
	}

	public void Move(float amount) {
			bpAnimator.SetBool("isWalking",true);
			if (!body.getUsesSpaceGravity()) {
				characterController.Move(amount);
			}
	}


	public void CancelChargingSpaceJump(){
		GUIManager.activatePlayingGUIWithFadeIn ();
		//GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().returnOriginalZ();
		if (GameManager.getActualPlanetIsRelevant ()) {
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().returnOriginalZ ();
		} else {
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setObjectiveZCameraSmallPlanet();
		}
		ParticleSystem particles = particleSystemJumpCharge.GetComponent<ParticleSystem> ();
		particles.Stop ();
		bpAnimator.SetBool("isChargingSpaceJumping",false);
		isChargingSpaceJump = false;
		HideArrow ();
	}

	public void Poison(int totalHitPoints, float timeBetweenTicks) {
		StartCoroutine (doPoisonTicks(totalHitPoints, timeBetweenTicks));
	}

	private IEnumerator doPoisonTicks(int totalHitPoints, float timeBetweenTicks) {
		this.GetComponent<OutlineChanging> ().setMainColor (Color.green);
		while (totalHitPoints > 0) {
			getHurt(1,GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass);
			totalHitPoints--;
			yield return new WaitForSeconds (timeBetweenTicks);
		}
		this.GetComponent<OutlineChanging> ().resetMainColor ();
		yield return null;
	}

	public void StopMove() {
		bpAnimator.SetBool("isWalking",false);
		characterController.StopMoving ();
	}

	public void ChargeJump() {
		Planet planet = body.getClosestPlanet ();
		if(planet!=null){
			if(planet.isPlanetCorrupted()){
				PlanetCorrupted pc = (PlanetCorrupted) planet;
				if(pc.getPlanetEventsManager()!=null){
					pc.getPlanetEventsManager().chargeSpaceJumping();
				}
			}
		}
		GUIManager.deactivatePlayingGUI ();
		bpAnimator.SetBool("isChargingSpaceJumping",true);
		isChargingSpaceJump = true;
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setObjectiveZCameraOnSpaceJump ();
		ParticleSystem particles = particleSystemJumpCharge.GetComponent<ParticleSystem> ();
		particles.Play ();
		StopMove ();
		ShowArrow ();
	}

	public void getHurt(int hitPointsToSubstract,Vector3 positionImpact){
		if (!isInvulnerable && !attackController.isDoingBlock () && !GameManager.isGameEnded && !GameManager.isGamePaused) {
			getHurtBigPappada.transform.position = positionImpact;
			getHurtBigPappada.GetComponent<ParticleSystem>().Play();
			if(!getIsSpaceJumping()){
				//fallDown();
			}
			GameManager.playerAnimator.SetTrigger("isHurt");
			GetComponent<DialogueController>().createNewExpression("Ouch!",0.5f,true,true);
			GameManager.audioManager.PlayStableSound(8);
			GUIManager.getHurtEffect ();
			killable.TakeDamage (hitPointsToSubstract);
			pappadaC.newProportionOfLife (killable.proportionHP ());
			if (killable.HP <= 0 && !GameManager.isGameEnded) {
				onDieCallEvent();
				GameManager.playerAnimator.SetBool("isDerribado",true);
				StopMove();
				isInvulnerable = true;
				GameManager.audioManager.StopSong();
				GameManager.audioManager.PlayStableSound(9);
				StartCoroutine(dissolveAndLose());
			}
		}
		StartCoroutine ("takeHit");
	}

	private void onDieCallEvent(){
		if (body.getClosestPlanet () != null) {
			if(body.getClosestPlanet().GetComponent<Planet>().isPlanetCorrupted()){
				PlanetCorrupted pc = body.getClosestPlanet().GetComponent<PlanetCorrupted>();
				if(pc.getPlanetEventsManager()!=null){
					pc.getPlanetEventsManager().playerDies();
				}
			}
		}
	}

	private IEnumerator dissolveAndLose(){
		GameManager.inputController.disableInputController ();
		bpAnimator.gameObject.layer = LayerMask.NameToLayer ("Dashing");
		yield return new WaitForSeconds (0.5f);
		particlesOnDematerialize.GetComponent<ParticleSystem> ().Play ();
		float timer = 0f;
		float timeToDissolve = 1f;
		while(timer<timeToDissolve){
			timer+=Time.deltaTime;
			float ratio = timer/timeToDissolve;
			GetComponent<Dissolve>().setDisolution(1f-ratio);
			yield return null;
		}
		isDematerialized = true;

		GameManager.loseGame ();
	}

	private IEnumerator rematerialize(){

		isInvulnerable = true;
		yield return new WaitForSeconds (1f);
		particlesOnMaterialize.GetComponent<ParticleSystem> ().Play ();
		yield return new WaitForSeconds (0.2f);
		float timer = 0f;
		float timeToDissolve = 1f;
		while(timer<timeToDissolve){
			timer+=Time.deltaTime;
			float ratio = timer/timeToDissolve;
			GetComponent<Dissolve>().setDisolution(ratio);
			yield return null;
		}
		isDematerialized = false;
		isInvulnerable = false; 
		bpAnimator.gameObject.layer = LayerMask.NameToLayer ("Player");
		GameManager.inputController.enableInputController();

		if (GameManager.playerSpaceBody.getClosestPlanet ()!=null) {
			if(GameManager.playerSpaceBody.getClosestPlanet().isPlanetCorrupted()){
				PlanetCorrupted pc = (PlanetCorrupted)GameManager.playerSpaceBody.getClosestPlanet();
				if(pc.getPlanetEventsManager()!=null){
					pc.getPlanetEventsManager().playerRespawned();
				}
			}
		}
	}

	//Method that makes the player fall, becoming invulnerable for a while
	public void fallDown(){
		attackController.interruptActualAttacks ();
		StopMove ();
		StartCoroutine (fallDownCoroutine ());
	}

	private IEnumerator fallDownCoroutine(){
		float timer = 0f;
		isFallingDown = true;
		isInvulnerable = true;
		GetComponent<OutlineChanging> ().setMainColor (Color.black);
		bpAnimator.SetBool ("isFallingDown", true);
		GetComponent<Rigidbody> ().AddForce (transform.up*10f, ForceMode.VelocityChange);
		while(timer<invulnerableTimeOnFallDown){
			timer+=Time.deltaTime;

			yield return null;
		}
		bpAnimator.SetBool ("isFallingDown", false);
		isFallingDown = false;
		GetComponent<OutlineChanging> ().setMainColor (Color.yellow);
		yield return new WaitForSeconds (invulnerableTimeAfterFallDown);
		GetComponent<OutlineChanging> ().resetMainColor ();
		isInvulnerable = false;


	}

	public void gainLife(int lifeToGain){
		killable.GainHealth (lifeToGain);
	}

	public void kill(){
		getHurt(killable.HP,GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass);
		StopMove ();
	}

	public bool isHit() {
		return gotHit;
	}

	IEnumerator takeHit() {
		gotHit = true;
		yield return new WaitForSeconds (1f);	
		gotHit = false;
	}

	public void MoveArrow(float horizontalMove,float verticalMove){
		Vector3 horizontalDirection = transform.forward * horizontalMove * Time.deltaTime * 20f;
		if(!characterController.getIsLookingRight()){
			horizontalDirection *= -1f;
		}
		Vector3 newPositionLine = (lineJumpDirection +(0.05f * horizontalDirection )).normalized;

		if(Vector3.Angle(transform.up,newPositionLine)<70){
			lineJumpDirection = newPositionLine;
			ActArrow();
		}
	}

	public void ActArrow(){
		lineRenderer.SetPosition (0, transform.position);
		lineRenderer.SetPosition(1,transform.position + (lineJumpDirection * lineJumpDistance));
		lineRenderer.SetWidth (2f,2f);
	}

	public void ShowArrow(){
		lineJumpDirection = transform.up;
		isShowingLineJump = true;
	}

	public void HideArrow(){
		lineRenderer.SetPosition (0, transform.position);
		lineRenderer.SetPosition (1, transform.position);
		isShowingLineJump = false;
	}

	public void reset(){
		if(killable!=null){
			killable.resetHP ();
			pappadaC.newProportionOfLife(killable.proportionHP());
		}
		initializeVariables ();
	}

	public bool getIsSpaceJumping(){
		return isSpaceJumping;
	}

	public bool getIsLookingRight(){
		return characterController.getIsLookingRight();
	}

	public bool getIsJumping(){
		return characterController.getIsJumping();
	}

	public Animator getAnimator(){
		if(bpAnimator==null){
			bpAnimator = animationBigPappada.GetComponent<Animator>();
		}
		return bpAnimator;
	}

	public bool getIsChargingSpaceJump(){
		return isChargingSpaceJump;
	}

	public void setCanDrownInSpace(bool cdis){
		canDrownInSpace = cdis;
	}

	public bool getIsFallingDown(){
		return isFallingDown;
	}
}
