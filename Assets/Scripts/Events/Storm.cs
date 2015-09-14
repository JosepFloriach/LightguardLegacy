using UnityEngine;
using System.Collections;

public class Storm : MonoBehaviour {

	public float stormFrecuency = 5.0f; 
	public float stormDuration = 4.0f; 
	public int stormDamage = 2; 
	public int hailEmissionRate = 400;
	public int stormEmissionRate = 200;
	public float damageEveryXSeconds = 1.5f;
	public LayerMask toCollide;  
	public GameObject hailRockPrefab;

	private float timeElapsedOff = 0f; 
	private float timeElapsedOn = 0f; 
	private float timeElapsedLastHit = 0f; 
	private bool interruptor = false; 
	private bool isHailing = false;
	private ParticleSystem ps; 

	// Use this for initialization
	void Start () {
		timeElapsedLastHit = damageEveryXSeconds;
		ps = GetComponent<ParticleSystem> ();
		//ps.Stop ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.player.transform.parent != null) {
			bool isInsideIceFirePlanet = GameManager.player.transform.parent.name.Equals ("Central Planet");
			if (isInsideIceFirePlanet && !GetComponentInParent<FrostFirePlanetEventsManager>().runnerActivated) {
				stormSwitch ();
				/*if (isHailing) {
					if (!Physics.Raycast (GameManager.player.transform.position, GameManager.player.transform.up, 100, toCollide)) {
						timeElapsedLastHit += Time.deltaTime; 
						if (timeElapsedLastHit >= damageEveryXSeconds) {
							timeElapsedLastHit = 0f; 
							GameManager.playerController.getHurt (stormDamage, GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass);
						}				
					}
				}*/
			}
		}
	}

	private void stormSwitch() {
		if (GameManager.getIsInsidePlanet ()) {
			ps.Stop ();
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().stopCameraShaking();
		} else {
			if(!ps.isPlaying){
				ps.Play();
			}
			if (!isHailing) {
				timeElapsedOff = timeElapsedOff + Time.deltaTime;
				if (timeElapsedOff >= stormFrecuency) {
					ps.emissionRate = hailEmissionRate;
					timeElapsedOff = 0f; 
					timeElapsedOn = 0f; 
					isHailing = true;
					StartCoroutine(spawnRocks());
					GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().setCameraShaking();
				}
			}
			
			if (isHailing) {
				timeElapsedOn = timeElapsedOn + Time.deltaTime;
				if (timeElapsedOn >= stormDuration) {
					ps.emissionRate = stormEmissionRate;
					timeElapsedOff = 0f; 
					timeElapsedOn = 0f; 
					isHailing = false;
					GameManager.mainCamera.GetComponent<CameraFollowingPlayer>().stopCameraShaking();
				}
			}
		}
	}

	private IEnumerator spawnRocks(){
		float timer = 0f;
		while(isHailing && !GameManager.getIsInsidePlanet ()){
			timer+=Time.deltaTime;
			if(timer>=0.05f){
				timer = 0f;

				Vector3 position = GameManager.player.transform.up * 50f;
				float angleRotated = (Random.value -0.5f) * 90f;
				position = Quaternion.Euler(new Vector3(0f,0f,Random.value*angleRotated))*position;
				position += transform.position;
				position.z = GameManager.player.transform.position.z;
				bool isGoodPosition = true;
				Vector3 direction = transform.position - position;
				Collider[] colliders = Physics.OverlapSphere(position,1.5f);
				if(colliders.Length>1){
					isGoodPosition = false;
				}
				
				if(isGoodPosition){
					GameObject platform = GameObject.Instantiate(hailRockPrefab) as GameObject;
					platform.transform.parent = transform;
					platform.transform.position = position;
				}
			}
			yield return null;
		}
	}
	
}
