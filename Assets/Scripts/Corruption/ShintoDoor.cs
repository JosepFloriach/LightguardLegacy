using UnityEngine;
using System.Collections;

public class ShintoDoor : MonoBehaviour {

	public GameObject KanjiLevel1;
	public GameObject KanjiLevel2;
	public GameObject KanjiLevel3;

	int actualLevelChanging = 0;
	private bool isMovingCameraToShinto = false;
	private bool isMovingCameraToPlayer = false;
	private bool isActivatingKanji = false;
	private bool isWaitingLookingAtKanji = false;

	private float timeWaiting = 0f;
	Vector3 cameraOriginalRotation;
	Vector3 cameraOriginalPosition;

	Vector3 centerOfPlanet;


	// Use this for initialization
	void Start () {

		centerOfPlanet = transform.parent.position;

	}

	public void disableKanjis(){
		KanjiLevel1.SetActive (false);
		KanjiLevel2.SetActive (false);
		KanjiLevel3.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void activateKanjis(){
		KanjiLevel1.SetActive (true);
		KanjiLevel2.SetActive (true);
		KanjiLevel3.SetActive (true);
	}
}
