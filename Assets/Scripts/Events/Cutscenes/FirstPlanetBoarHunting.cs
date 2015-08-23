using UnityEngine;
using System.Collections;

public class FirstPlanetBoarHunting : Cutscene {

	public GameObject boar;

	public override void Initialize() {
		identifyier = CutsceneIdentifyier.FirstPlanetBoarHunting;
		boar.GetComponent<IAController> ().init ();
		boar.GetComponent<IAController> ().deactivate ();
		boar.GetComponent<CharacterController> ().LookLeftOrRight (-1f);
		(boar.GetComponent<CharacterAttackController> ().getAttack (AttackType.BoarChargeAttack) as BoarChargeAttack).timeBeforeCharge = 0f;
		(boar.GetComponent<CharacterAttackController> ().getAttack (AttackType.BoarChargeAttack) as BoarChargeAttack).chargeSpeed = 5f;
		(boar.GetComponent<CharacterAttackController> ().getAttack (AttackType.BoarChargeAttack) as BoarChargeAttack).chargeParticles.GetComponent<ParticleSystem> ().Stop ();
	}

	public void makeBoarGoAway (){
		StartCoroutine("boarHunting");
	}

	private IEnumerator boarHunting(){
		
		boar.GetComponent<CharacterController> ().LookLeftOrRight(1f);
		boar.GetComponent<IAController> ().Move (1f);
		yield return new WaitForSeconds(3f);
		boar.SetActive (false);
	}
	
	public override void ActivateTrigger() {
	}


}
