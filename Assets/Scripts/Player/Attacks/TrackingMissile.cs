using UnityEngine;
using System.Collections;

public class TrackingMissile : MonoBehaviour {
	


	//GameObjects
	public GameObject elementalParticleEffect;

	//Public Variables
	public float basStartSpeed = 5.5f;
	public float speedTimeMultiplier = 1.2f;
	public float angleTimeIncrement = 0.05f;
	public float explosionMaxScale = 1f;
	public float timeExplode = 0.8f;
	public float timeToStayMaxSize = 0.3f;

	//Private Variables
	private Vector3 speed;
	private float totalspeed = 2.5f;
	private float timeHasBeenAlive;
	private bool hasArrived = false;
	private float timerWhenArrived;
	private TrackingMissilesAttack originalAttack;
	private GameObject objective;
	private bool isDying = false;


	void OnTriggerEnter(Collider other) {

			if(other.tag.Equals("Enemy")){
				originalAttack.enemyHit(other.gameObject);
				//Destroy(gameObject);
				isDying = true;
			}else if(other.tag.Equals("Planet")){
				//Destroy(gameObject);
				isDying = true;
			}

	}

	public void initialize(Vector3 vectorUp,GameObject objective,TrackingMissilesAttack originalAttack,ElementType elementType){
		timeHasBeenAlive = 0f;
		this.objective = objective;
		this.originalAttack = originalAttack;
		speed = new Vector3 (Random.value, Random.value,0f).normalized;
		speed = ((speed * 0.8f) + vectorUp).normalized;

		if(!elementType.Equals(ElementType.None)){
			elementalParticleEffect.SetActive(true);
			elementalParticleEffect.GetComponent<ParticleSystem>().Play();
			Material material = AttackElementsManager.getElement(elementType).material;
			if(material!=null){
				elementalParticleEffect.GetComponent<ParticleSystemRenderer >().material = material;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		timeHasBeenAlive += Time.deltaTime;
		if(isDying){
			if(!hasArrived){
				hasArrived = true;
				timerWhenArrived = 0f;
			}
			timerWhenArrived+=Time.deltaTime;
			float ratio = timerWhenArrived/timeExplode;
			if(ratio>=1f){ratio = 1f;}
			ratio = explosionMaxScale * ratio;
			transform.localScale = new Vector3(ratio,ratio,ratio);
			if(timerWhenArrived>=timeExplode){
				ratio = (timerWhenArrived-timeExplode)/timeToStayMaxSize;
				/*if(GetComponent<Renderer>().material.HasProperty("_TintColor")){
					Color originalColor = GetComponent<Renderer>().material.GetColor("_TintColor");
					Color newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f-ratio);
					GetComponent<Renderer>().material.SetColor("_TintColor",newColor);
				}
				foreach(Renderer r in GetComponentsInChildren<Renderer>()){
					if(r.material.HasProperty("_TintColor")){
						Color originalColor = r.material.GetColor("_TintColor");
						Color newColor = new Color(originalColor.r,originalColor.g,originalColor.b,1f-ratio);
						r.material.SetColor("_TintColor",newColor);
					}
				}*/
				if(timerWhenArrived>=timeToStayMaxSize+timeExplode){
					Destroy(gameObject);
				}
			}
			
		}else{
			totalspeed = basStartSpeed + (timeHasBeenAlive * speedTimeMultiplier);
			transform.position = transform.position+(speed * Time.deltaTime * totalspeed);
			if(objective!=null){
				Vector3 objectiveDirection = objective.transform.position -  transform.position;
				int angleRotation =(int) (timeHasBeenAlive/angleTimeIncrement);
				speed = Vector3.RotateTowards(speed,objectiveDirection,Mathf.Deg2Rad * angleRotation,1f).normalized;
			}
		
		}
	
	}
}
