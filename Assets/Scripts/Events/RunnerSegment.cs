using UnityEngine;
using System.Collections;

[System.Serializable]
public class RunnerSegment {
	public bool resetPlatforms = false;
	public bool movePlatforms = false;
	public float speedPlatforms = 0f;

	public float startingScale;
	public float endingScale;

	public float timeItLasts;

	public float startingFireRotation;
	public float endingFireRotation;
	public Checkpoint segmentCheckpoint;

}
