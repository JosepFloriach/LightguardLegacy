using UnityEngine;
using System.Collections;

public class SlashController : MonoBehaviour {

	public GameObject slashPrefab;

	public void doSlash(){
		GameObject newSlash = GameObject.Instantiate (slashPrefab);
		newSlash.transform.position = transform.position;
		newSlash.transform.rotation = transform.rotation;

		newSlash.GetComponent<Slash> ().initialize (transform.right);
	}
}
