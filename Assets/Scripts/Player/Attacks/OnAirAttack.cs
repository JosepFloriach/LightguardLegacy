using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OnAirAttack : Attack {
	
	public GameObject triggerBox;
	public float positionInFrontPlayer = 0.5f;


	private List<GameObject> enemiesHit;
	private bool hasHitEnemy;
	// Use this for initialization

	public override void initialize(){
		attackType = AttackType.OnAir;
		enemiesHit = new List<GameObject> (0);
	}

	public override void enemyCollisionEnter(GameObject enemy,Vector3 point){
		if(!enemiesHit.Contains(enemy) && !enemy.GetComponent<IAController>().isDead){
			enemiesHit.Add(enemy);
			enemy.GetComponent<IAController>().getHurt(1,point,false);
			enemy.GetComponent<IAController>().hitCanSendFlying();
			//GameManager.audioManager.PlayStableSound(10);
			if(!hasHitEnemy){
				hasHitEnemy = true;
				GameManager.lightGemEnergyManager.addPoints(1);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.player.GetComponent<GravityBody>().getIsTouchingPlanetOrCharacters()){
			isFinished = true;
			triggerBox.SetActive(false);
		}
	}

	private IEnumerator airAttack(){

		triggerBox.transform.position = GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass + GameManager.player.transform.forward.normalized * positionInFrontPlayer;
		triggerBox.SetActive (true);
		GameManager.audioManager.PlaySound (3);
		triggerBox.transform.parent = GameManager.player.transform;
		triggerBox.transform.rotation = GameManager.player.transform.rotation;
		yield return true;
		isFinished = true;

	}

	public override void startAttack(){
		if(isFinished){
			isFinished = false;
			enemiesHit.Clear();
			hasHitEnemy = false;
			GameManager.playerAnimator.SetTrigger("isDoingAirAttack");
			StartCoroutine ("airAttack");
		}
	}
}
