using UnityEngine;
using System.Collections;

public class AnimationPacker : MonoBehaviour {

	CharacterAttackController atkControl;

	// Use this for initialization
	//Clase temporal, porque BigP con la animacion debe ser el objeto base, actualmente es 
	
	void Start () {
		atkControl = GetComponentInParent<CharacterAttackController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void enableHitbox() {
	print("Hit Box!!!");
	
	}
	
	public void disalbeHitbox() {}
	
}
