using UnityEngine;
using System.Collections;

public class Killable:MonoBehaviour {

	public int HP = 3;

	private int maxHP;
	// Use this for initialization
	void Start () {
		maxHP = HP;
		//renderer.material.color = new Color (0.0f, 1.0f, 0.0f);

	}
	
	public void TakeDamage (int AttackPoints) {
		
		HP -= AttackPoints;
		
	}

	public void GainHealth(int healthAmmount){
		if(healthAmmount+HP>maxHP){
			HP = maxHP;
		}else{
			HP+=healthAmmount;
		}
	}

	public void resetHP(){
		HP = maxHP;
	}
	
	IEnumerator Fade() {
		for (float f = 1f; f >= 0; f -= 0.1f) {
			//Color c = renderer.material.color;
			//c.a = f;
			//renderer.material.color = c;
			yield return null;
		}
	}

	public float proportionHP(){
		if(HP == maxHP){return 1f;}
		return (float)HP / (float)maxHP;
	}

	public bool isDead(){
		if(HP<=0){return true;}
		else{return false;}
	}
	
}
