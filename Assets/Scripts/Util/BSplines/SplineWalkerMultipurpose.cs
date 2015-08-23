using UnityEngine;

public class SplineWalkerMultipurpose : MonoBehaviour {

	public BezierSpline spline;

	public float duration;

	public bool lookForward;

	public SplineWalkerMode mode;
	public bool independentDeltaTime = false;

	public float startProgress = 0f;
	private float progress;
	private bool goingForward = true;
	private float lastTime ;
	private float deltaTime;


	void Start(){
		progress = startProgress;
		lastTime = Time.realtimeSinceStartup;
	}

	private void Update () {
		if(independentDeltaTime){
			deltaTime = Time.realtimeSinceStartup - lastTime;
		}else{
			deltaTime = Time.deltaTime;
		}
		lastTime = Time.realtimeSinceStartup;


		if (goingForward) {
			progress += deltaTime / duration;
			if (progress > 1f) {
				if (mode == SplineWalkerMode.Once) {
					progress = 1f;
				}
				else if (mode == SplineWalkerMode.Loop) {
					progress -= 1f;
				}
				else {
					progress = 2f - progress;
					goingForward = false;
				}
			}
		}
		else {
			progress -= deltaTime / duration;
			if (progress < 0f) {
				progress = -progress;
				goingForward = true;
			}
		}

		Vector3 position = spline.GetPoint(spline.getTByLength(progress));
		transform.position = position;
		if (lookForward) {
			transform.LookAt(position + spline.GetDirection(progress),transform.forward);
		}
	}
}