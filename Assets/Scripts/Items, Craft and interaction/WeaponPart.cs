using UnityEngine;
using System.Collections;

public class WeaponPart : Item {

	public enum Position { REAR, MIDDLE, FRONT };

	private string name;
	private float damage;
	private float distance;
	private bool areaDamage;
	private Position side;
	private float speed;

	public float GetDamage() {
		return damage;
	}
	public float GetDistance() {
		return distance;
	}
	public bool IsAreaDamage() {
		return areaDamage;
	}
	public float GetSpeed() {
		return speed;
	}
	public string GetName() {
		return name;
	}
	public Position GetPosition() {
		return side;
	}

	public void SetDamage(float value) {
		damage = value;
	}
	public void SetDistance(float value) {
		distance = value;
	}
	public void SetAreaDamage(bool value) {
		areaDamage = value;
	}
	public void SetSpeed(float value) {
		speed = value;
	}
	public void SetName(string value) {
		name = value;
	}
	public void SetPosition(string position) {
		if (position == "front") {side = Position.FRONT;}
		if (position == "middle") {side = Position.MIDDLE;}
		if (position == "rear") {side = Position.REAR;}
	}
}
