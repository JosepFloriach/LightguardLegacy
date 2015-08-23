using UnityEngine;
using System.Collections;

public class SplineNavigator : MonoBehaviour {

	public BezierSpline spline;
	public float time = 3f;
	public bool invert = false;

	private float originalMagnitudeSpeed = 0f;
	private bool canStart = true;
	float originalZ = 0f;

	void OnTriggerEnter(Collider collider){
		if(collider.tag.Equals("Player") && canStart && GameManager.playerSpaceBody.getCanEnterDragonSlide()){
			GameManager.playerSpaceBody.setCanEnterDragonSlide(false);
			canStart = false;
			originalZ = GameManager.player.transform.position.z;
			originalMagnitudeSpeed = Mathf.Abs(GameManager.player.GetComponent<Rigidbody>().velocity.magnitude);
			GameManager.player.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
			GameManager.playerController.setCanDrownInSpace(false);
			if(GameManager.player.GetComponent<SplineWalker> ()==null){
				GameManager.player.AddComponent<SplineWalker> ();
			}else{
				GameManager.player.GetComponent<SplineWalker> ().enabled = true;
			}
			GameManager.player.GetComponent<SplineWalker> ().spline = spline;
			GameManager.player.GetComponent<SplineWalker> ().duration = time;
			GameManager.player.GetComponent<SplineWalker> ().lookForward = false;
			if (invert) {
				GameManager.player.GetComponent<SplineWalker> ().setInvert();
			}else{
				GameManager.player.GetComponent<SplineWalker> ().setNonInvert();
			}
			GameManager.audioManager.PlayStableSound(12);
			StartCoroutine(removeAfterTime(time));
		}
	}

	IEnumerator removeAfterTime(float time){
		float timer = 0f;
		while(!GameManager.player.GetComponent<SplineWalker> ().isFinished()){
			timer+= Time.deltaTime;
			yield return null;
		}
		GameManager.playerController.setCanDrownInSpace(true);
		GameManager.player.GetComponent<SplineWalker> ().enabled = false;
		canStart = true;
		Vector3 newVelocity;
		if (invert) {
			newVelocity = (spline.getFirstDirection().normalized) * (originalMagnitudeSpeed);
		}else{
			newVelocity = (spline.getLastDirection().normalized) * (originalMagnitudeSpeed);
		}
		newVelocity.z = 0f;
		GameManager.player.GetComponent<Rigidbody> ().velocity = newVelocity;
		GameManager.player.transform.position = new Vector3 (GameManager.player.transform.position.x, GameManager.player.transform.position.y, originalZ);
	}
}
