using UnityEngine;
using System.Collections;

public enum ElementType{None,Ice,Fire,Lightning,Earth}

public class Element : MonoBehaviour {
	protected ElementType type;

	public Material material;
	
	void Start(){
		initialize ();
	}
	
	void Update(){
		update ();
	}

	public virtual void doEffect(GameObject characterAffected){

	}
	
	protected virtual void initialize(){
		
	}
	
	protected virtual void update(){
		
	}

	public ElementType getType(){
		return type;
	}
}
