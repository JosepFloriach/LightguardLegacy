using UnityEngine;
using System.Collections;

public class Storm : MonoBehaviour {

	public float stormFrecuency = 5.0f; 
	public float stormDuration = 4.0f; 
	public int stormDamage = 2; 
	public float damageEveryXSeconds = 1.5f;
	public LayerMask toCollide;  

	private float timeElapsedOff = 0f; 
	private float timeElapsedOn = 0f; 
	private float timeElapsedLastHit = 0f; 
	private bool interruptor = false; 
	private ParticleSystem ps; 

	// Use this for initialization
	void Start () {
		timeElapsedLastHit = damageEveryXSeconds;
		ps = GetComponent<ParticleSystem> ();
		ps.Stop ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.player.transform.parent != null) {
			bool isInsideIceFirePlanet = GameManager.player.transform.parent.name.Equals ("Central Planet");
			if (isInsideIceFirePlanet && !GetComponentInParent<FrostFirePlanetEventsManager>().runnerActivated) {
				stormSwitch ();
				if (ps.isPlaying) {
					if (!Physics.Raycast (GameManager.player.transform.position, GameManager.player.transform.up, 100, toCollide)) {
						timeElapsedLastHit += Time.deltaTime; 
						if (timeElapsedLastHit >= damageEveryXSeconds) {
							timeElapsedLastHit = 0f; 
							GameManager.playerController.getHurt (stormDamage, GameManager.player.GetComponent<Rigidbody> ().worldCenterOfMass);
						}				
					}
				}
			}
		}
	}

	private void stormSwitch() {
		if (GameManager.getIsInsidePlanet ()) {
			ps.Stop ();
		} else {
			if (ps.isStopped) {
				timeElapsedOff = timeElapsedOff + Time.deltaTime;
				if (timeElapsedOff >= stormFrecuency) {
					ps.Play ();
					timeElapsedOff = 0f; 
					timeElapsedOn = 0f; 
				}
			}
			
			if (ps.isPlaying) {
				timeElapsedOn = timeElapsedOn + Time.deltaTime;
				if (timeElapsedOn >= stormDuration) {
					ps.Stop ();
					timeElapsedOff = 0f; 
					timeElapsedOn = 0f; 
				}
			}
		}
	}
}
