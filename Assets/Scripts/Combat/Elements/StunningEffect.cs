using UnityEngine;
using System.Collections;

public class StunningEffect : MonoBehaviour {
	
	public float timeItLasts = 1.5f;
	
	
	private float timer = 0f;
	private GameObject elementStunning;
	
	public void initialize(GameObject elementToStun){
		elementStunning = elementToStun;
		timer = 0f;
		GetComponent<ParticleSystem> ().Play ();
		transform.parent = elementStunning.transform;
		transform.position = elementStunning.transform.position;
		Vector3 auxEuler = transform.eulerAngles;
		transform.up = elementStunning.transform.up;
		transform.Rotate (auxEuler);
		float height = elementStunning.GetComponentInChildren<Renderer> ().bounds.extents.y;
		transform.position = transform.position + (height * elementStunning.transform.up * 1.5f);
		elementStunning.GetComponent<IAController> ().stun (timeItLasts);
		elementStunning.GetComponent<IAController> ().interruptAttack ();
	}
	
	void Update(){
		timer += Time.deltaTime;
		if(timer>=timeItLasts){
			Destroy(gameObject);
		}
		
	}
}
