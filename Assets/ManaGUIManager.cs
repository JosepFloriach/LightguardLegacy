using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class ManaGUIManager : MonoBehaviour {

	public static ManaGUIManager instance { get; private set; }
	
	private int currentMana = 4;
	
	public static ManaGUIManager GetInstance() {
		if (instance == null) {
			instance = GameObject.FindObjectOfType<ManaGUIManager>();
		}
		return instance;
	}

	void Start () {
		resetManaPoints ();
	}

	void Update() {
		float newEnergyNumber = GameManager.lightGemEnergyManager.pointsPercentage();
		if (newEnergyNumber == 1f) {
			transform.Find ("Fruit4").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
			transform.Find ("Fruit3").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
			transform.Find ("Fruit2").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
			transform.Find ("Fruit1").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
		} else if (newEnergyNumber > 0.75f) {
			transform.Find ("Fruit4").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit3").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
			transform.Find ("Fruit2").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
			transform.Find ("Fruit1").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
		} else if (newEnergyNumber > 0.50f) {
			transform.Find ("Fruit4").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit3").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit2").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
			transform.Find ("Fruit1").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
		} else if (newEnergyNumber > 0.25f) {
			transform.Find ("Fruit4").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit3").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit2").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit1").GetComponent<Image> ().color = new Color(180f,180f,180f,0.50f);
		} else {
			transform.Find ("Fruit4").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit3").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit2").GetComponent<Image> ().color = Color.clear;
			transform.Find ("Fruit1").GetComponent<Image> ().color = Color.clear;
		}
	}

	public void resetManaPoints() {
		string strsprite;
		for (int i = 1; i < 5; i++) {
			strsprite =  "Fruit" + i.ToString (); 
			Image img = transform.Find (strsprite).GetComponent<Image>();
			img.color = new Color(180f,180f,180f,0.50f);
		}
		currentMana = 4; 
	}
}
