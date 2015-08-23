using UnityEngine;
using System.Collections;

public class BlendShapeController : MonoBehaviour {
	
	SkinnedMeshRenderer skinnedMeshRenderer;
	float blendOne = 0f;
	float posSpeed = 1f;
	float negSpeed = -1f;
	float speed;
	
	void Awake ()
	{
		skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer> ();
	}
	
	void Start ()
	{
		speed = posSpeed;
	}
	
	void Update ()
	{
		if (blendOne > 100f) {
			speed = negSpeed;
		}
		
		if (blendOne < 0f) {
			speed = posSpeed;
		}
		blendOne += speed;	
		skinnedMeshRenderer.SetBlendShapeWeight (0, blendOne);
		
	}
	
}
