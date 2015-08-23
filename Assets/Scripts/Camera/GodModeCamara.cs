using UnityEngine;
using System.Collections;

public class GodModeCamara : MonoBehaviour {
	
	public bool areYouGod = false;
	public float wheelspeed = 5f;
	public float godspeed = 0.5f;
	// Use this for initialization
	void Start () {
		
	}

	private ElementType transformElement(ElementType et){
		if (et.Equals (ElementType.Earth)) {
			return ElementType.Fire;
		} else if (et.Equals (ElementType.Fire)) {
			return ElementType.Ice;
		} else if (et.Equals (ElementType.Ice)) {
			return ElementType.Lightning;
		} else if (et.Equals (ElementType.Lightning)) {
			return ElementType.None;
		} else if (et.Equals (ElementType.None)) {
			return ElementType.Earth;
		}
		return ElementType.None;
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Period)) {
			if (areYouGod) {
				areYouGod = false;
				this.gameObject.GetComponent<CameraFollowingPlayer> ().enabled = true;
				GameManager.player.GetComponent<InputController> ().enabled = true;
			} else {
				areYouGod = true;
				this.gameObject.GetComponent<CameraFollowingPlayer> ().enabled = false;
				GameManager.player.GetComponent<InputController> ().enabled = false;
			}
		}else if (Input.GetKeyDown (KeyCode.Alpha0)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(0).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha1)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(1).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(2).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(3).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(4).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(5).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(6).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha7)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(7).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}else if (Input.GetKeyDown (KeyCode.Alpha8)) {
			GameManager.player.transform.position = GameManager.checkPointManager.getCheckpointByIndex(8).checkPointObject.transform.position;
			Vector3 cameraPosition = GameManager.player.transform.position;
			cameraPosition.z = GameManager.mainCamera.transform.position.z;
			GameManager.mainCamera.transform.position = cameraPosition;
		}

	
		if (areYouGod) {
			float xmove = Input.GetAxis ("Horizontal");
			float ymove = Input.GetAxis ("Vertical");
			float zmove = Input.GetAxis ("Mouse ScrollWheel");
			//TODO: Restore the correct position in Z
			this.transform.position += new Vector3 (xmove * godspeed, ymove * godspeed, zmove * wheelspeed);
			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				transform.position = new Vector3(0.0f,0.0f,transform.position.z);
			}
			
			
			
		}
		
		
	}
}
