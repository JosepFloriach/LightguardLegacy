using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour {

	public float dashSpeed = 30f;
	public float dashTime = 0.3f;
	public GameObject dashStartParticles;
	public LayerMask layerToDash;
	public float dashCooldown = 1f;
	public LayerMask maskToCollide;
	private bool cooldownFinished = true;
	private bool isDoingDash = false;


	Vector3 originalMovement;

	public bool getIsDoingDash(){
		return isDoingDash;
	}

	public bool isCooldownFinished(){
		return cooldownFinished;
	}

	IEnumerator doDash(){
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Enemy"),true);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("EnemyAttack"),true);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Dashing"),true);
		isDoingDash = true;
		cooldownFinished = false;

		dashStartParticles.SetActive (true);
		dashStartParticles.GetComponent<ParticleSystem> ().Play ();
		dashStartParticles.transform.position = GameManager.player.GetComponent<Rigidbody>().worldCenterOfMass;
		originalMovement = GameManager.player.GetComponent<CharacterController>().getMoveAmout ();
		GameManager.playerAnimator.SetTrigger("isDashing");
		Vector3 newMove;
		GameManager.playerSpaceBody.setHasToApplyForce (false);
		newMove = (dashSpeed) * GameManager.player.transform.forward;
		GameManager.player.GetComponent<CharacterController> ().setSpeed (dashSpeed);

		float distance = dashSpeed * dashTime;
		//Raycast 
		RaycastHit hit;

		
		float shortestDistance = GameManager.playerController.centerToExtremesDistance;

		float dashTimer = 0f;
		bool collision = false;
		while (dashTimer<dashTime) {
			dashTimer+=Time.deltaTime;

			Vector3 velocity = GameManager.player.GetComponent<Rigidbody> ().velocity;
			Vector3 forward = GameManager.player.transform.forward;
			Vector3 direction = (velocity + (forward * dashSpeed));
			Vector3 bodyPosition = GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass;
			//Vector3 feetPosition = GameManager.player.transform.position + (GameManager.player.transform.up *0.01f);

			if (Physics.Raycast (bodyPosition, direction, out hit, shortestDistance + 0.1f, layerToDash)) {
				collision = true;
				break;
			}

			/*if (Physics.Raycast (feetPosition, direction, out hit, shortestDistance + 0.1f, layerToDash)) {
				collision = true;
				break;
			}*/
			yield return null;
		}
		
		isDoingDash = false;
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Enemy"),false);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("EnemyAttack"),false);
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Dashing"),false);
		GameManager.player.GetComponent<CharacterController> ().resetSpeed ();
		if(collision){
			GameManager.player.GetComponent<CharacterController> ().setAmount(0f);
		}
		GameManager.playerAnimator.SetBool("isWalking",false);
		GameManager.playerSpaceBody.setHasToApplyForce (true);
		yield return new WaitForSeconds(dashCooldown);
		dashStartParticles.GetComponent<ParticleSystem> ().Stop ();
		cooldownFinished = true;
		GameManager.playerAnimator.ResetTrigger("isDashing");
	}

	public void startAction(){
		StartCoroutine ("doDash");
	}
}
