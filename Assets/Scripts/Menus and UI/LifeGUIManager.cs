using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class LifeGUIManager : MonoBehaviour {
	//public static LifeGUIManager instance { get; private set; }

	public GameObject[] lifeLeafs;
	public float updateTime = 0.05f;
	private Color leafsColor;
	private float timer = 0f;

	void Awake(){
		if (lifeLeafs.Length > 0) {
			leafsColor = lifeLeafs [0].GetComponent<Image> ().color;
		}
	}
	//private int currentSprite = 1;

	/*public static LifeGUIManager GetInstance() {
		if (instance == null) {
			instance = GameObject.FindObjectOfType<LifeGUIManager>();
		}
		return instance;
	}*/

	void LateUpdate(){
		timer += Time.deltaTime;
		if (timer > updateTime) {
			updateUI();
			timer = 0f;
		}
	}

	void updateUI(){
		float lifeProportion = GameManager.player.GetComponent<Killable> ().proportionHP ();
		int elementsToShow = (int)(lifeLeafs.Length * lifeProportion);
		for (int i = 0; i<lifeLeafs.Length; ++i) {
			if(i>=elementsToShow){
				lifeLeafs[i].GetComponent<Image>().color = Color.clear;
			}else{
				lifeLeafs[i].GetComponent<Image>().color = leafsColor;
			}
		}
	}



	/*public void resetHitPoints() {
		string strsprite;
		for (int i = 1; i < 8; i++) {
			strsprite =  "Leaf" + i.ToString (); 
			Image img = transform.Find (strsprite).GetComponent<Image>();
			img.color = new Color(180f,180f,180f,0.50f);
		}
		currentSprite = 1; 
	}*/
	
	/*public void addHitPoint() {
		string strsprite = "Leaf" + currentSprite.ToString (); 
		if (currentSprite < 8) {
			Debug.Log(strsprite);
			transform.Find (strsprite).GetComponent<Image> ().color = Color.clear;
			currentSprite++;
		}
	}*/
}
