using UnityEngine;

public class SplineWalker : MonoBehaviour {

	public BezierSpline spline;

	public float duration;

	public bool lookForward;

	public SplineWalkerMode mode;
	public bool independentDeltaTime = false;

	private float progress = 0f;
	private bool goingForward = true;
	private bool invertDirection = false;
	private float lastTime ;
	private float deltaTime;

	void Start(){
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
				progress = 1f;
			}
		}else{
			progress -= deltaTime / duration;
			if (progress < 0f) {
				progress = 0f;
			}
		}

		Vector3 position = spline.GetPoint(spline.getTByLength(progress));
		transform.localPosition = position;
		if (lookForward) {
			transform.LookAt(position + spline.GetDirection(progress));
		}
	}

	public bool isFinished(){
		if(!invertDirection){
			if(progress >= 1f){
				return true;
			}
		}else if(invertDirection){
			if(progress <= 0f){
				return true;
			}
		}
		return false;
	}

	public void restart(){
		progress = 0f;
		goingForward = true;
	}

	public void setInvert(){
		invertDirection = true;
		progress = 1f;
		goingForward = false;
	}

	public void setNonInvert(){
		invertDirection = false;
		progress = 0f;
		goingForward = true;
	}
}