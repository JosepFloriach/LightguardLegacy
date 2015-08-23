using UnityEngine;
using System.Collections;

public class PappadaController : MonoBehaviour {

	public float maxPappadaXScale;
	public float minPappadaXScale;

	public float maxPappadaYScale;
	public float minPappadaYScale;

	public float maxPappadaZScale;
	public float minPappadaZScale;


	// Use this for initialization
	void Start () {
		newProportionOfLife (1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void newProportionOfLife(float proportion){
		float scaleX = (maxPappadaXScale - minPappadaXScale) * proportion;
		scaleX += minPappadaXScale;
		if(scaleX<minPappadaXScale){scaleX = minPappadaXScale;}

		float scaleY = (maxPappadaYScale - minPappadaYScale) * proportion;
		scaleY += minPappadaYScale;
		if(scaleY<minPappadaYScale){scaleY = minPappadaYScale;}

		float scaleZ = (maxPappadaZScale - minPappadaZScale) * proportion;
		scaleZ += minPappadaZScale;
		if(scaleZ<minPappadaZScale){scaleZ = minPappadaZScale;}

		transform.localScale = new Vector3 (scaleX, scaleY, scaleZ);

		//transform.renderer.material.color = new Color (1f - proportion, 0f, 0f);
	}
}
