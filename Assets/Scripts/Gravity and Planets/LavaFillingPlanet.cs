using UnityEngine;
using System.Collections;

public class LavaFillingPlanet : MonoBehaviour {

	public GameObject corePlanet;
	public ParticleSystem particleSystem;

	public float timeToGrow = 2f;

	public Vector3 maxScale;

	public float offset = 0f;


	public float timeToStayMaxGrown = 2f;
	public float timeToStayMinGrown = 5f;

	private bool offsetCounted = false;
	private float timer = 0f;
	private enum Phase{Growing,WaitingGrown,Ungrowing,WaitingUngrown};
	private Phase actualPhase = Phase.Growing;

	void Awake(){
		if(particleSystem!=null){
			particleSystem.Stop ();
		}
	}

	void Update(){
		timer += Time.deltaTime;
		if (timer >= offset && !offsetCounted) {
			timer = 0f;
			offsetCounted = true;
		}
		if (offsetCounted) {
			if (actualPhase.Equals (Phase.Growing)) {
				float ratio = timer / timeToGrow;
				Vector3 scale = maxScale * ratio;
				corePlanet.transform.localScale = scale;
				if (timer > timeToGrow) {
					nextPhase ();
				}

			} else if (actualPhase.Equals (Phase.WaitingGrown)) {
				float ratio = timer / timeToStayMaxGrown;
				//Vector3 scale = maxScale * (ratio);
				if (timer > timeToStayMaxGrown) {
					if(particleSystem!=null){
						particleSystem.Stop();
					}
					nextPhase ();
				}

			} else if (actualPhase.Equals (Phase.Ungrowing)) {
				float ratio = timer / timeToGrow;
				Vector3 scale = maxScale * (1f - ratio); 
				corePlanet.transform.localScale = scale;
				if (timer > timeToGrow) {
					nextPhase ();
				}

			} else if (actualPhase.Equals (Phase.WaitingUngrown)) {
				float ratio = timer / timeToStayMinGrown;
				//Vector3 scale = maxScale * ratio;
				if (timer > timeToStayMinGrown) {
					nextPhase ();
				}else{
					if(particleSystem!=null){
						if(timeToStayMinGrown-timer < 0.75f && !particleSystem.isPlaying){
							particleSystem.Play();
						}
					}
				}

			}
		}
	}

	private void nextPhase(){
		if(actualPhase.Equals(Phase.Growing)){
			actualPhase = Phase.WaitingGrown;
		}else if(actualPhase.Equals(Phase.WaitingGrown)){
			actualPhase = Phase.Ungrowing;
		}else if(actualPhase.Equals(Phase.Ungrowing)){
			actualPhase = Phase.WaitingUngrown;
		}else if(actualPhase.Equals(Phase.WaitingUngrown)){
			actualPhase = Phase.Growing;
		}
		timer = 0f;
	}

}
