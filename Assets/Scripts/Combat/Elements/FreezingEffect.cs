using UnityEngine;
using System.Collections;

public class FreezingEffect : MonoBehaviour {
	
	public float timeItLasts = 2f;
	
	
	private float timer = 0f;
	private GameObject elementFreezing;
	
	public void initialize(GameObject elementToFreeze){
		elementFreezing = elementToFreeze;
		timer = 0f;
		GetComponent<ParticleSystem> ().Play ();
		transform.parent = elementFreezing.transform;
		transform.position = elementFreezing.transform.position;
		Vector3 auxEuler = transform.eulerAngles;
		transform.up = elementFreezing.transform.up;
		transform.Rotate (auxEuler);
		elementFreezing.GetComponent<IAController> ().freeze (timeItLasts);
		//elementFreezing.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
	}
	
	void Update(){
		timer += Time.deltaTime;
		elementFreezing.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
		if(timer>=timeItLasts){
			Destroy(gameObject);
		}
		
	}

}
