using UnityEngine;
using System.Collections;

public class WeaponColider : MonoBehaviour {

	public int damage = 1;
	public Vector3 direction;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Enemy") {
			IAController enemy = other.GetComponent<IAController>();
			enemy.getHurt(damage,other.ClosestPointOnBounds(transform.position),true);
		}
	}
	
}
