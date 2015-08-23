using UnityEngine;
using System.Collections;

public class BurningEffect : MonoBehaviour {

	public float timeItLasts = 2f;
	public float timeBetweenTicks = 0.4f;
	public int damagePerTick = 1;


	private float timer = 0f;
	private float tickTimer = 0f;
	private GameObject elementBurning;

	public void initialize(GameObject elementToBurn){
		elementBurning = elementToBurn;
		timer = 0f;
		tickTimer = 0f;
		GetComponent<ParticleSystem> ().Play ();
		transform.parent = elementBurning.transform;
		transform.position = elementBurning.transform.position;
	}

	void Update(){
		timer += Time.deltaTime;
		tickTimer += Time.deltaTime;

		if(timer>=timeItLasts){
			Destroy(gameObject);
		}else if(tickTimer>=timeBetweenTicks){
			tickTimer = 0f;
			elementBurning.GetComponent<IAController>().getHurt(damagePerTick,transform.position,true);
		}

	}


}
