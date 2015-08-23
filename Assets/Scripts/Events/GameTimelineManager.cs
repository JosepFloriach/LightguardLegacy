using UnityEngine;
using System.Collections;

public class GameTimelineManager : MonoBehaviour {

	bool hasUpdated = false;
	void Update () {

		if(!hasUpdated){
			gameBegins();
			hasUpdated = true;
		}
	}

	void gameBegins(){
		//It's the start of the game
		GameManager.rebuildGameFromGameState ();
		GUIManager.deactivatePlayingGUI();
		GameManager.inputController.disableInputController ();
		GUIManager.activateMenu(GameManager.actualSceneManager.startingMenu);
		GUIManager.fadeAllIn();

		//Get the actual planet and initialize it
		Planet planet = GameManager.playerSpaceBody.getClosestPlanet ();
		if(planet!=null && planet.isPlanetCorrupted()){
			(planet as PlanetCorrupted).getPlanetEventsManager().activate();
		}
	}

}
