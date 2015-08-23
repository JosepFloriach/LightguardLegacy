using UnityEngine;
using System.Collections;

public class RecipesContentMenuManager : MonoBehaviour {

	public GameObject[] recipes;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void activateRecipe(GameObject recipe){
		foreach(GameObject recipeO in recipes){
			recipeO.SetActive(false);
		}
		recipe.SetActive (true);
	}
}
