using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShockwaveAttack : Attack,AnimationSubscriber {

	//GameObjects
	public GameObject areaEffect;
	public GameObject chargeEffect;
	public GameObject elementalParticlesOnCharge;
	public GameObject elementalParticlesOnExplode;

	//Public Variables
	public float timeItDoesAttack = 0.8f;
	public float endScaleOfAttack = 4f;
	public float startChargeScale = 1f;
	public float forceExplosion = 5f;
	public float timeToCharge = 0.8f;

	//Private Variables
	private float timer;
	private AnimationEventBroadcast eventHandler;
	private List<GameObject> enemiesHit;

	public override void enemyCollisionEnter(GameObject enemy,Vector3 point){
		//If it's an enemy we damage him
		if(!enemiesHit.Contains(enemy) && !enemy.GetComponent<IAController>().isDead){
			enemiesHit.Add(enemy);
			enemy.GetComponent<IAController>().getHurt(damage,enemy.transform.position,true);
			float radius = areaEffect.GetComponent<SphereCollider> ().radius * endScaleOfAttack;
			Vector3 position = areaEffect.transform.position;
			enemy.GetComponent<Rigidbody>().velocity = ((enemy.transform.position - chargeEffect.transform.position).normalized + enemy.transform.up).normalized*forceExplosion;
			GameManager.comboManager.addCombo ();
			if(!elementAttack.Equals(ElementType.None)){
				AttackElementsManager.getElement(elementAttack).doEffect(enemy);
			}
		}
	}

	public override void initialize(){
		attackType = AttackType.Shockwave;
		eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
	}

	private IEnumerator doShockwave(){
		enemiesHit = new List<GameObject> (0);
		areaEffect.transform.localScale = new Vector3(0f,0f,0f);
		chargeEffect.transform.localScale = new Vector3(startChargeScale,startChargeScale,startChargeScale);
		areaEffect.transform.position = GameManager.player.transform.position;
		chargeEffect.transform.position = GameManager.player.transform.position;
		chargeEffect.SetActive (true);
		if(!elementAttack.Equals(ElementType.None)){
			elementalParticlesOnCharge.SetActive(true);
			elementalParticlesOnCharge.transform.position = chargeEffect.transform.position;
			elementalParticlesOnCharge.GetComponent<ParticleSystem>().Play();
			Material material = AttackElementsManager.getElement(elementAttack).material;
			if(material!=null){
				elementalParticlesOnCharge.GetComponent<ParticleSystemRenderer >().material = material;
			}
		}
		timer = 0f;
		while (timer<timeToCharge) {
			timer += Time.deltaTime;
			float ratio = (1f - ((timer) / (timeToCharge))) * startChargeScale;
			chargeEffect.transform.localScale = new Vector3 (ratio, ratio, ratio);
			chargeEffect.transform.position = GameManager.playerController.lightGemObject.transform.position;
			elementalParticlesOnCharge.transform.position = GameManager.playerController.lightGemObject.transform.position;
			yield return null;
		}

		elementalParticlesOnCharge.SetActive (false);
		if(!elementAttack.Equals(ElementType.None)) {
			elementalParticlesOnExplode.SetActive(true);
			elementalParticlesOnExplode.transform.position = areaEffect.transform.position;
			elementalParticlesOnExplode.GetComponent<ParticleSystem>().Play();
			Material material = AttackElementsManager.getElement(elementAttack).material;
			if(material!=null){
				elementalParticlesOnExplode.GetComponent<ParticleSystemRenderer >().material = material;
			}
		}
		areaEffect.SetActive (true);
		chargeEffect.SetActive (false);
		GameManager.playerAnimator.SetBool("isDoingShockwave",true);

		timer = 0f;
		while(timer<timeItDoesAttack){
			timer += Time.deltaTime;
			float ratio = (timer/(timeItDoesAttack)) * endScaleOfAttack;
			areaEffect.transform.localScale = new Vector3(ratio,ratio,ratio);
			areaEffect.transform.position = GameManager.playerController.lightGemObject.transform.position;
			elementalParticlesOnExplode.transform.position = GameManager.playerController.lightGemObject.transform.position;
			yield return null;
		}

		isFinished = true;
		GameManager.playerAnimator.SetBool("isDoingShockwave",false);
		areaEffect.SetActive (false);
		elementalParticlesOnExplode.SetActive (false);
	}


	public override void startAttack(){
		GameManager.playerAnimator.SetTrigger("isChargingShockwave");
		isFinished = false;
	}

	
	void AnimationSubscriber.handleEvent(string idEvent) {
		Debug.Log (idEvent);
		switch (idEvent) {
		case "chargeStart": 
			StartCoroutine("doShockwave");
			break;
		case "chargeEnd":
			break;
		default: 
			
			break;
		}
	}
	
	
	string AnimationSubscriber.subscriberName() {
		return  "Shockwave";	
	}
}
