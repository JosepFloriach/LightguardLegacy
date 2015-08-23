using UnityEngine;
using System.Collections;

public class BlendOnGettingCloser : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPosition = GameManager.player.transform.position;
		SphereCollider sphereCollider = (SphereCollider)transform.GetComponent<Collider>();
		float distance = (GameManager.player.transform.position - transform.position).magnitude;
		float radius = sphereCollider.radius * transform.lossyScale.x;
		float playerAthmosphereDistance = 1f-(distance/radius);//Mathf.Abs(((playerPosition - transform.position).magnitude - (radius/2))/(radius/2));
		//Debug.Log (playerAthmosphereDistance);

		if (playerAthmosphereDistance < -1f) {
			playerAthmosphereDistance = Mathf.Abs(playerAthmosphereDistance+1f);
		}else if (playerAthmosphereDistance < (1/3.5f)) {
			playerAthmosphereDistance = playerAthmosphereDistance * 3.5f;
		} else {
			playerAthmosphereDistance = 1f;
		}

		//Debug.Log (playerAthmosphereDistance);

		Color originalColour = GetComponent<Renderer>().material.color;
		//renderer.material.color  = new Color(originalColour.r,originalColour.g,originalColour.b, playerAthmosphereDistance);
	}
}
