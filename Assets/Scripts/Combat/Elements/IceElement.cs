using UnityEngine;
using System.Collections;

public class IceElement : Element {

	public GameObject freezingEffectPrefab;

	protected override void initialize(){
		type = ElementType.Ice;
	}
	
	protected override void update(){
		
	}
	
	public override void doEffect(GameObject characterAffected){
		GameObject freezingEffect = Instantiate(freezingEffectPrefab) as GameObject;
		freezingEffect.GetComponent<FreezingEffect> ().initialize (characterAffected);
	}
}
