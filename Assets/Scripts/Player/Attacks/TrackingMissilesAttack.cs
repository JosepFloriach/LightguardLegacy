using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackingMissilesAttack : Attack,AnimationSubscriber {

	//GameObjects
	public GameObject attackCharge;
	public GameObject elementalParticlesOnCharge;
	public GameObject missilePrefab;
	public GameObject kameUp;
	public float range = 8f;

	//Public variables
	public float timeToChargeAttack = 0.7f;
	public int maxAmmountOfMissilesSpawned = 4;
	public float startChargeScale = 1f;
	public float timeItGrows = 0.4f;
	public float maxScaleGrow = 2f;
	public float timeToStayMaxGrowth = 0.2f;

	//Private variables
	private AnimationEventBroadcast eventHandler;
	public float speedUp = 2f;
	public float timeGoingUp = 0.3f;
	private List<GameObject> enemiesHit;
	private bool canDoNewAttack = true;
	private float timer = 0f;

	public override void initialize(){
		attackType = AttackType.Missiles;
		eventHandler = GameManager.playerAnimator.gameObject.GetComponent<AnimationEventBroadcast>();
		eventHandler.subscribe(this);
	}

	public void enemyHit(GameObject enemy){
		if(!enemiesHit.Contains(enemy) && !enemy.GetComponent<IAController>().isDead){
			enemiesHit.Add(enemy);
			enemy.GetComponent<IAController>().getHurt(damage,(enemy.transform.position),true);
			GameManager.comboManager.addCombo ();
			if(!elementAttack.Equals(ElementType.None)){
				AttackElementsManager.getElement(elementAttack).doEffect(enemy);
			}
		}
	}

	private void spawnMissiles(Vector3 position){

		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		List<GameObject> enemiesInRange = new List<GameObject> (0);
		foreach(GameObject enemy in enemies){
			if(Vector3.Distance(enemy.transform.position,attackCharge.transform.position)<=range){
				enemiesInRange.Add(enemy);
			}
		}

		int ammountOfMissiles = maxAmmountOfMissilesSpawned;
		if(enemiesInRange.Count<ammountOfMissiles){
			ammountOfMissiles = enemiesInRange.Count;
		}

		for(int i = 0;i<ammountOfMissiles;++i){
			GameObject newMissile = Instantiate(missilePrefab);
			newMissile.transform.position = position;
			newMissile.GetComponent<TrackingMissile>().initialize(GameManager.player.transform.up,enemiesInRange[i],this,elementAttack);
		}
	}

	IEnumerator doTrackingMissilesAttack(){
		enemiesHit = new List<GameObject> (0);
		attackCharge.SetActive (true);
		attackCharge.transform.position = GameManager.playerController.lightGemObject.transform.position ;
		timer = 0f;
		if (!elementAttack.Equals (ElementType.None)) {
			elementalParticlesOnCharge.SetActive(true);
			elementalParticlesOnCharge.GetComponent<ParticleSystem>().Play();
			Material material = AttackElementsManager.getElement(elementAttack).material;
			if(material!=null){
				elementalParticlesOnCharge.GetComponent<ParticleSystemRenderer >().material = material;
			}
		}
		timer = 0f;
		while(timer<=timeToChargeAttack){
			timer+=Time.deltaTime;
			float ratio = (1f - ((timer) / (timeToChargeAttack))) * startChargeScale;
			attackCharge.transform.localScale = new Vector3 (ratio, ratio, ratio);
			attackCharge.transform.position = GameManager.playerController.lightGemObject.transform.position;
			elementalParticlesOnCharge.transform.position = GameManager.playerController.lightGemObject.transform.position;
			attackCharge.transform.position = GameManager.playerController.lightGemObject.transform.position ;
			yield return null;
		}
		attackCharge.SetActive (false);
		elementalParticlesOnCharge.SetActive (false);


		Vector3 up = GameManager.player.transform.up;
		kameUp.SetActive (true);
		kameUp.transform.position = attackCharge.transform.position;
		timer = 0f;
		float originalScale = kameUp.transform.localScale.x;
		while(timer<timeGoingUp){
			timer+=Time.deltaTime;
			float ratio = timer/timeGoingUp;
			float scale = ((maxScaleGrow - originalScale) * ratio)+originalScale;
			kameUp.transform.localScale = new Vector3(scale,scale,scale);
			kameUp.transform.position += up.normalized*Time.deltaTime*speedUp;
			yield return null;
		}
		GameManager.playerAnimator.SetBool("isDoingMissiles",false);
		isFinished = true;
		timer = 0f;
		spawnMissiles(kameUp.transform.position);
		while (timer<timeToStayMaxGrowth) {
			timer+=Time.deltaTime;
			float ratio = timer/timeToStayMaxGrowth;
			/*if(kameUp.GetComponent<Renderer>().material.HasProperty("_TintColor")){
				Color originalColor = kameUp.GetComponent<Renderer>().material.GetColor("_TintColor");
				Color newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f-ratio);
				kameUp.GetComponent<Renderer>().material.SetColor("_TintColor",newColor);
			}
			foreach(Renderer r in kameUp.GetComponentsInChildren<Renderer>()){
				if(r.material.HasProperty("_TintColor")){
					Color originalColor = r.material.GetColor("_TintColor");
					Color newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f-ratio);
					r.material.SetColor("_TintColor",newColor);
				}
			}*/
			yield return null;
		}
		/*if(kameUp.GetComponent<Renderer>().material.HasProperty("_TintColor")){
			Color originalColor = kameUp.GetComponent<Renderer>().material.GetColor("_TintColor");
			Color newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f);
			kameUp.GetComponent<Renderer>().material.SetColor("_TintColor",newColor);
		}
		foreach(Renderer r in kameUp.GetComponentsInChildren<Renderer>()){
			if(r.material.HasProperty("_TintColor")){
				Color originalColor = r.material.GetColor("_TintColor");
				Color newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f);
				r.material.SetColor("_TintColor",newColor);
			}
		}*/
		kameUp.transform.localScale = new Vector3(originalScale,originalScale,originalScale);
		kameUp.SetActive (false);
		canDoNewAttack = true;


	}

	public override bool canDoNextAttack()
	{
		return canDoNewAttack;
	}

	public override void startAttack(){
		GameManager.playerAnimator.SetBool("isDoingMissiles",true);
		isFinished = false;
		canDoNewAttack = false;
	}

	void AnimationSubscriber.handleEvent(string idEvent) {
		switch (idEvent) {
		case "start": 
			if(!isFinished){
				StartCoroutine ("doTrackingMissilesAttack");
			}
			break;
		case "end":
			break;
		default: 
			break;
		}
		
	}

	string AnimationSubscriber.subscriberName() {
		return  "TrackingMissiles";	
	}
}
