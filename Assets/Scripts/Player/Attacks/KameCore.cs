using UnityEngine;
using System.Collections;

public class KameCore : MonoBehaviour {

	public GameObject kameAttack;
	private KameAttackDirectionable kameAttackDirectionable;
	
	void OnTriggerEnter(Collider collider){
		if(kameAttackDirectionable==null){
			kameAttackDirectionable = kameAttack.GetComponent<KameAttackDirectionable> ();
		}
		kameAttackDirectionable.centerHit (collider.gameObject);
	}

}
