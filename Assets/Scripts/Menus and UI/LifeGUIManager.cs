using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class LifeGUIManager : MonoBehaviour {
	//public static LifeGUIManager instance { get; private set; }

	public GameObject[] lifeLeafs;
	public float updateTime = 0.05f;
	public Color originalColor;
	private Color leafsColor;
	private float timer = 0f;

	void Awake(){
		resetUI ();
	}
	/*void LateUpdate(){
		timer += Time.deltaTime;
		if (timer > updateTime) {
			updateUI();
			timer = 0f;
		}
	}*/

	public void updateUI(){
		float lifeProportion = GameManager.player.GetComponent<Killable> ().proportionHP ();
		int hitPoint = (int)(lifeLeafs.Length * lifeProportion);
		lifeLeafs [hitPoint].GetComponent<VanishEffect> ().activateVanishEffect ();
	}


	public void resetUI() {
		if (lifeLeafs.Length > 0) {
			for (int i = 0; i<lifeLeafs.Length; ++i) {
				lifeLeafs[i].GetComponent<Image>().color = originalColor;
				lifeLeafs[i].transform.localScale = new Vector2(1f,1f);
				lifeLeafs[i].GetComponent<VanishEffect>().deactivateVanishEffect();
			}
		}
	}
}
