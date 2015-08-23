using UnityEngine;
using System.Collections;

public class CameraMinimap : MonoBehaviour {

	public bool rotateWithPlayer = true;
	void Awake(){
		GameManager.registerMinimapCamera (gameObject);
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 objectivePosition = new Vector3 (GameManager.player.transform.position.x, GameManager.player.transform.position.y, transform.position.z);
		transform.position = objectivePosition;
		if(rotateWithPlayer){
			transform.rotation = Quaternion.LookRotation (Vector3.forward, GameManager.player.transform.up);
		}else{
			transform.rotation = Quaternion.LookRotation (Vector3.forward,Vector3.up);
		}
	}
}
