using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class LifeGUIManager : MonoBehaviour {
	//public static LifeGUIManager instance { get; private set; }

	public GameObject[] lifeLeafs;
	public Color originalColor;

	void Awake(){
		resetUI ();
	}

	public void addHitPoints(int hitPoints){
		float lifeProportion = GameManager.player.GetComponent<Killable> ().proportionHP ();
		int startHitPoint = (int)(lifeLeafs.Length * lifeProportion);
		int endHitPoint = startHitPoint - hitPoints-1;
		int i = startHitPoint-1;
		while (i > endHitPoint && i>=0) {
			lifeLeafs[i].GetComponent <VanishEffect>().activateVanishEffect();
			i--;
		}
	}

	public void healHitPoints(int healPoints) {
		float lifeProportion = GameManager.player.GetComponent<Killable> ().proportionHP ();
		int startHealPoint = (int)(lifeLeafs.Length * lifeProportion);
		int endHealPoint = startHealPoint + healPoints;
		int i = startHealPoint;
		while (i < endHealPoint && i <= GameManager.player.GetComponent<Killable> ().getMaxHP()) {
			lifeLeafs[i].GetComponent <VanishEffect>().activateRevertEffect();
			i++;
		}
	}

	public void resetUI() {
		if (lifeLeafs.Length > 0) {
			for (int i = 0; i<lifeLeafs.Length; ++i) {
				lifeLeafs[i].GetComponent<Image>().color = originalColor;
				lifeLeafs[i].transform.localScale = new Vector2(1f,1f);
				lifeLeafs[i].GetComponent<VanishEffect>().deactivateVanishEffect();
				lifeLeafs[i].GetComponent<VanishEffect>().deactivateRevertVanishEffect();
			}
		}
	}
}
