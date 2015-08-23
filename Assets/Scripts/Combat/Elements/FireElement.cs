using UnityEngine;
using System.Collections;

public class FireElement : Element {

	public GameObject burningEffectPrefab;

	protected override void initialize(){
		type = ElementType.Fire;
	}
	
	protected override void update(){
		
	}
	
	public override void doEffect(GameObject characterAffected){
		GameObject burningEffect = Instantiate(burningEffectPrefab) as GameObject;
		burningEffect.GetComponent<BurningEffect> ().initialize (characterAffected);
	}
}
