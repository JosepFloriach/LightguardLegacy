using UnityEngine;
using System.Collections;

public class LightGemEnergyManager : MonoBehaviour {

	public int maxLightPoints;
	public int actualLightPoints;
	public float pointsRegeneratedPerSecond;

	private float timeToRegeneratePoint;
	private float timer;

	public bool canDoSpecialAttack(int attackPoints){
		if(attackPoints<=actualLightPoints){
			return true;
		}else{
			return false;
		}
	}

	public float pointsPercentage(){
		return (float)actualLightPoints / (float)maxLightPoints;
	}
	public void addPoints(int points){
		if(actualLightPoints+points>maxLightPoints){
			actualLightPoints = maxLightPoints;
		}else{
			actualLightPoints+=points;
		}
	}

	public void substractPoints(int points){
		if(actualLightPoints-points>0){
			actualLightPoints-=points;
		}else{
			actualLightPoints = 0;
		}
	}

	// Use this for initialization
	void Start () {
		if(pointsRegeneratedPerSecond==0f){
			timeToRegeneratePoint = float.PositiveInfinity;
		}else{
			timeToRegeneratePoint = 1f / pointsRegeneratedPerSecond;
		}
		timer = 0f;
		GameManager.registerLightGemEnergyManager(gameObject);
	}
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if(timer>=timeToRegeneratePoint){
			timer = 0f;
			addPoints(1);
		}
	}
}
