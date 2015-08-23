using UnityEngine;
using System.Collections;

public class LightningElement : Element {

	public GameObject lightningEffectPrefab;

	protected override void initialize(){
		type = ElementType.Lightning;
	}
	
	protected override void update(){
		
	}
	
	public override void doEffect(GameObject characterAffected){
		GameObject lightningElement = Instantiate(lightningEffectPrefab) as GameObject;
		lightningElement.GetComponent<LightningEffect> ().initialize (characterAffected);
	}
}
