using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Animation))]

public class AnimationController : MonoBehaviour {

	private Animation animation;
	private bool isAttacking;

	// Use this for initialization
	void Start () {
		animation = GetComponent <Animation>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsAttacking() {
		return animation.IsPlaying ("Attack");
	}

	public bool IsWalking() {
		return animation.IsPlaying ("Walk");
	}

	public void Attack() {
		animation.Play ("Attack");
	}

	public void StopAttack() {
		animation.Stop ("Attack");
	}

	public void Walk() {
		animation.Play ("Walk");
	}

	public void StopWalk() {
		animation.Stop ("Walk");
	}
}
