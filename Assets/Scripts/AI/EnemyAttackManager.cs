using UnityEngine;
using System.Collections;

public class EnemyAttackManager : MonoBehaviour {

	public int attackBudgetMax = 100;
	public int budgetRegenerationPerSecond = 30;

	private float budgetRegenerationTimer = 0f;
	private int attackBudget = 0;

	void Awake(){
		GameManager.registerEnemyAttackManager (gameObject);
	}
	// Use this for initialization
	void Start () {
	
	}

	public bool canPayAttack(int cost){
		if(attackBudget>cost){
			return true;
		}else{
			return false;
		}
	}

	public void doNewAttack(int cost){
		attackBudget -= cost;
	}
	
	// Update is called once per frame
	void Update () {
		budgetRegenerationTimer += Time.deltaTime;
		if(budgetRegenerationTimer>=1f){
			budgetRegenerationTimer = 0f;
			if((attackBudget+budgetRegenerationPerSecond)>=attackBudgetMax){
				attackBudget = attackBudgetMax;
			}else{
				attackBudget+=budgetRegenerationPerSecond;
			}
		}

	
	}


}
