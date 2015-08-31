using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class LifeGUIManager : MonoBehaviour {
	public static LifeGUIManager instance { get; private set; }

	private int currentSprite = 1;

	public static LifeGUIManager GetInstance() {
		if (instance == null) {
			instance = GameObject.FindObjectOfType<LifeGUIManager>();
		}
		return instance;
	}

	public void resetHitPoints() {
		string strsprite;
		for (int i = 1; i < 8; i++) {
			strsprite =  "Leaf" + i.ToString (); 
			Image img = transform.Find (strsprite).GetComponent<Image>();
			img.color = new Color(180f,180f,180f,0.50f);
		}
		currentSprite = 1; 
	}
	
	public void addHitPoint() {
		string strsprite = "Leaf" + currentSprite.ToString (); 
		if (currentSprite < 8) {
			Debug.Log(strsprite);
			transform.Find (strsprite).GetComponent<Image> ().color = Color.clear;
			currentSprite++;
		}
	}
}
