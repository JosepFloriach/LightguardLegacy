using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;


public class WeaponManager : MonoBehaviour {
	public enum Position { REAR, MIDDLE, FRONT };

	private static WeaponManager singleton;

	private WeaponPart currentRearPart;
	private WeaponPart currentMiddlePart;
	private WeaponPart currentFrontPart;

	private Dictionary<string, WeaponPart> allPosibleParts;

	private Dictionary<string, WeaponPart> rearWeaponList;
	private Dictionary<string, WeaponPart> frontWeaponList;
	private Dictionary<string, WeaponPart> middleWeaponList;
	
	private string name;
	private float damage;
	private float distance;
	private bool areaDamage;
	private float speed;

	// Use this for initialization
	void Start () {
		allPosibleParts = new Dictionary<string, WeaponPart> ();
		rearWeaponList = new Dictionary<string, WeaponPart> ();
		frontWeaponList = new Dictionary<string, WeaponPart>();
		middleWeaponList = new Dictionary<string, WeaponPart>();
		LoadWeaponPartsFromXML ();
	}

	public static WeaponManager Instance {
		get{ return singleton ?? (singleton = new GameObject("Weaponmanager").AddComponent<WeaponManager>());}
	}	

	private void LoadWeaponPartsFromXML() {
		XmlDocument xmlDoc = new XmlDocument ();
		xmlDoc.Load ("Assets/Data/WeaponParts.xml");
		XmlNodeList weaponList = xmlDoc.GetElementsByTagName ("weapon");

		foreach (XmlNode weapon in weaponList) {
			XmlNodeList weaponParams = weapon.ChildNodes;
			WeaponPart newPart = new WeaponPart();
			foreach (XmlNode param in weaponParams){
				if (param.Name == "name"){newPart.SetName (param.InnerText);}
				if (param.Name == "distance") { newPart.SetDistance (float.Parse (param.InnerText));}
				if (param.Name == "damage") { newPart.SetDamage (float.Parse (param.InnerText));}
				if (param.Name == "area") { newPart.SetAreaDamage (bool.Parse (param.InnerText));}
				if (param.Name == "side") { newPart.SetPosition (param.InnerText);}
				if (param.Name == "speed"){ newPart.SetSpeed (float.Parse (param.InnerText));}
			}
			allPosibleParts.Add (newPart.GetName (), newPart);
		}
	}
	
	public void SetMiddlePart ( string name) {
		currentMiddlePart = middleWeaponList [name];
	}

	public void SetRearPart ( string name) {
		currentRearPart = middleWeaponList [name];
	}

	public void SetFrontPart ( string name) {
		currentFrontPart = middleWeaponList [name];
	}

	public WeaponPart GetEquippedRearPart() {
		return currentRearPart;
	}

	public WeaponPart GetEquippedFrontPart() {
		return currentFrontPart;
	}

	public WeaponPart GetEquippedMiddlePart() {
		return currentMiddlePart;
	}

	public void AddPart(WeaponPart item, Position sideToPlace) {
		if (sideToPlace ==  Position.FRONT ){
			frontWeaponList.Add (item.GetName(),item);
		}
		if (sideToPlace ==  Position.REAR ){
			frontWeaponList.Add (item.GetName(),item);
		}
		if (sideToPlace ==  Position.MIDDLE ){
			middleWeaponList.Add (item.GetName(),item);
		}
	}

	public float GetCurrentDamage() {
		return GetEquippedFrontPart ().GetDamage () + GetEquippedRearPart ().GetDamage () + GetEquippedMiddlePart ().GetDamage ();
	}

	public float GetCurrentDistance() {
		return GetEquippedFrontPart ().GetDistance () + GetEquippedRearPart ().GetDistance () + GetEquippedMiddlePart ().GetDistance ();
	}

	public bool IsAreaDamage() {
		return GetEquippedFrontPart ().IsAreaDamage() || GetEquippedRearPart ().IsAreaDamage() || GetEquippedMiddlePart ().IsAreaDamage ();
	}

	public float GetCurrentSpeed() {
		return GetEquippedFrontPart ().GetSpeed () + GetEquippedRearPart ().GetSpeed () + GetEquippedMiddlePart ().GetSpeed ();
	}

	public string GetCurrentName() {
		return GetEquippedFrontPart ().GetName () + GetEquippedMiddlePart ().GetName () + GetEquippedRearPart ().GetName ();
	}
}
