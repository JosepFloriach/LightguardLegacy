using UnityEngine;
using System.Collections;

public class HailInterruptor : MonoBehaviour {

	public float stormFrecuency = 5.0f; 
	public float stormDuration = 4.0f; 

	private float timeElapsedOff = 0f; 
	private float timeElapsedOn = 0f; 
	private bool interruptor = false; 

	// Use this for initialization
	void Start () {
		GetComponent<ParticleSystem> ().Stop ();
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem ps = GetComponent<ParticleSystem> ();
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
			if (timeElapsedOn >= stormDuration ) {
				ps.Stop ();
				timeElapsedOff = 0f; 
				timeElapsedOn = 0f; 
			}
		}
	}
}
