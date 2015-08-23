using UnityEngine;
using System.Collections;

public class EarthElement : Element {

	public GameObject stunningEffectPrefab;

	protected override void initialize(){
		type = ElementType.Earth;
	}

	protected override void update(){

	}

	public override void doEffect(GameObject characterAffected){
		GameObject stunningEffect = Instantiate(stunningEffectPrefab) as GameObject;
		stunningEffect.GetComponent<StunningEffect> ().initialize (characterAffected);
	}
}
