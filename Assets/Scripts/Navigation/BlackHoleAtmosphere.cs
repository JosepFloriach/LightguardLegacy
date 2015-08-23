using UnityEngine;
using System.Collections;

public class BlackHoleAtmosphere : MonoBehaviour {

	public GameObject blackHoleBody;

	public float rotationPerSecond;


	void Start(){
		float size = transform.GetComponent<SphereCollider> ().radius * Mathf.Max (transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
		size += blackHoleBody.GetComponent<GravityAttractor>().gravityDistance;
		
		//Athmosphere size
		float athmosphereSize = GetComponent<SphereCollider> ().radius * Mathf.Max (transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
		
		float factor = size / athmosphereSize;
		transform.localScale = new Vector3 (factor * transform.localScale.x, factor * transform.localScale.y, factor * transform.localScale.z);
	}

	void Update(){
		transform.Rotate(0f,0f,rotationPerSecond*Time.deltaTime);
	}


}
