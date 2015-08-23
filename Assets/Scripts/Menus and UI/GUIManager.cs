using UnityEngine;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Menu{None,
				MainMenu,
				ControlsMenu,
				CreditsMenu,
				YouWonMenu,
				YouLostMenu,
				StartingSplashScreen,
				CraftingMenu,
				InteractuablePopup,
				OnPauseMenu,
				BlackMenu,
				TutorialMenu,
				OptionsMenu,
				IntroScene
			    };

public class GUIManager : MonoBehaviour {

	public static FadeManager fadeManager;
	/*private bool weaponSmithRootMenu;
	private bool weaponCraftRootMenu;
	private bool itemCraftRootMenu;
	private bool inventory;
	private float scrwidth, scrheight;*/


	private static GameObject mainMenuO;
	private static GameObject controlsO;
	private static GameObject creditsO;
	private static GameObject youWonO;
	private static GameObject youLostO;
	private static GameObject startingSplashScreenO;
	private static GameObject craftingMenuO;
	private static GameObject interactuablePopupO;
	private static GameObject onPauseMenuO;
	private static GameObject playingGUIO;
	private static GameObject spaceJumpGUIO;
	private static GameObject blackMenuO;
	private static GameObject tutorialMenuO;
	private static GameObject optionsMenuO;
	private static GameObject introSceneO;
	private static GameObject rotatingOnPlanetMenuO;
	
	private static Text tutorialText;

	private static GameObject corruptionBar;
	private static GameObject minimapGUI;

	private static Menu nextMenu;

	private static GUIManager singleton;

	private static GameObject actualMenu;
	private static Menu actualMenuActivated = Menu.None;

	private static Action actionToDo;
	private static bool isSpaceGUIActive = false;

	private static List<FollowHighlightedButton> highlightFollowers;

	//Private 
	private static void deactivateMenus(){
		if(mainMenuO!=null){
			mainMenuO.SetActive(false);
		}
		if(introSceneO!=null){
			introSceneO.SetActive(false);
		}
		if(controlsO!=null){
			controlsO.SetActive(false);
		}
		if(creditsO!=null){
			creditsO.SetActive(false);
		}
		if(youLostO!=null){
			youLostO.SetActive(false);
		}
		if(youWonO!=null){
			youWonO.SetActive(false);
		}
		if(startingSplashScreenO!=null){
			startingSplashScreenO.SetActive(false);
		}
		if(craftingMenuO!=null){
			craftingMenuO.SetActive(false);
		}
		if(interactuablePopupO!=null){
			interactuablePopupO.SetActive(false);
		}
		if(onPauseMenuO!=null){
			onPauseMenuO.SetActive(false);
		}
		if(optionsMenuO!=null){
			optionsMenuO.SetActive(false);
		}
	}
	
	private static void changeMenuAndFadeIn(){
		deactivateMenus ();
		activateMenu (nextMenu);
		fadeManager.fadeIn(getMenu(nextMenu));
	}

	private static void changeMenuAndFadeInWithAction(){
		deactivateMenus ();
		activateMenu (nextMenu);
		fadeManager.fadeInWithAction(actionToDo,getMenu(nextMenu));
	}

	private static GameObject getMenu(Menu menu){
		if (menu.Equals (Menu.MainMenu)) {
			return mainMenuO;
		}else if(menu.Equals(Menu.IntroScene)){
			return introSceneO;
		}else if(menu.Equals(Menu.ControlsMenu)){
			return controlsO;
		}else if(menu.Equals(Menu.CreditsMenu)){
			return creditsO;
		}else if(menu.Equals(Menu.YouWonMenu)){
			return youWonO;
		}else if(menu.Equals(Menu.YouLostMenu)){
			return youLostO;
		}else if(menu.Equals(Menu.StartingSplashScreen)){
			return startingSplashScreenO;
		}else if(menu.Equals(Menu.CraftingMenu)){
			return craftingMenuO;
		}else if(menu.Equals(Menu.InteractuablePopup)){
			return interactuablePopupO;
		}else if(menu.Equals(Menu.OnPauseMenu)){
			return onPauseMenuO;
		}else if(menu.Equals(Menu.BlackMenu)){
			return blackMenuO;
		}else if(menu.Equals(Menu.OptionsMenu)){
			return optionsMenuO;
		}
		return null;
	}

	//Public
	public static GUIManager Instance {
		get{ return singleton ?? (singleton = new GameObject("GUIManager").AddComponent<GUIManager>());}
	}	

	private static GameObject allMenus;
	public static void initialize() {
		allMenus = new GameObject();
		allMenus.name = "All Menus Parent";
		
	}

	public static void registerMainMenu(GameObject mainMenuGO){
		if(mainMenuGO!=null && mainMenuO == null){
			mainMenuO = GameObject.Instantiate (mainMenuGO) as GameObject;
			mainMenuO.transform.SetParent(allMenus.transform);
			mainMenuO.SetActive (false);
		}
	}
	
	public static void registerIntroScene(GameObject introSceneGO) {
		if(introSceneGO!=null && introSceneO == null){
			introSceneO = GameObject.Instantiate (introSceneGO) as GameObject;
			introSceneO.transform.SetParent(allMenus.transform);
			introSceneO.SetActive (false);
		}
	
	}

	public static void registerControlsMenu(GameObject controlsGO){
		if(controlsGO!=null && controlsO == null){
			controlsO = GameObject.Instantiate (controlsGO) as GameObject;
			controlsO.transform.SetParent(allMenus.transform);
			controlsO.SetActive (false);
		}
	}

	
	public static void registerCreditsMenu(GameObject creditsGO){
		if(creditsGO!=null && creditsO == null){
			creditsO = GameObject.Instantiate (creditsGO) as GameObject;
			creditsO.transform.SetParent(allMenus.transform);
			creditsO.SetActive (false);
		}
	}

	public static void registerYouLostMenu(GameObject youLostGO){
		if(youLostGO!=null && youLostO == null){
			youLostO = GameObject.Instantiate (youLostGO) as GameObject;
			youLostO.transform.SetParent(allMenus.transform);
			youLostO.SetActive (false);
		}
	}

	public static void registerYouWonMenu(GameObject youWonGO){
		if(youWonGO!=null && youWonO == null){
			youWonO = GameObject.Instantiate (youWonGO) as GameObject;
			youWonO.transform.SetParent(allMenus.transform);
			youWonO.SetActive (false);
		}
	}

	public static void registerStartingSplashScreen(GameObject startingSplashScreenGO){
		if(startingSplashScreenGO!=null && startingSplashScreenO == null){
			startingSplashScreenO = GameObject.Instantiate (startingSplashScreenGO) as GameObject;
			startingSplashScreenO.transform.SetParent(allMenus.transform);
			startingSplashScreenO.SetActive (false);
		}
	}

	public static void registerCraftingMenu(GameObject craftingMenuGO){
		if(craftingMenuGO!=null && craftingMenuO == null){
			craftingMenuO = GameObject.Instantiate (craftingMenuGO) as GameObject;
			craftingMenuO.transform.SetParent(allMenus.transform);
			craftingMenuO.SetActive (false);
		}
	}

	public static void registerInteractuablePopup(GameObject interactuablePopupGO){
		if(interactuablePopupGO!=null && interactuablePopupO == null){
			interactuablePopupO = GameObject.Instantiate (interactuablePopupGO) as GameObject;
			interactuablePopupO.transform.SetParent(allMenus.transform);
			interactuablePopupO.SetActive (false);
		}
	}

	public static void registerOnPauseMenu(GameObject onPauseMenuGO){
		if(onPauseMenuGO!=null && onPauseMenuO == null){
			onPauseMenuO = GameObject.Instantiate (onPauseMenuGO) as GameObject;
			onPauseMenuO.transform.SetParent(allMenus.transform);
			onPauseMenuO.SetActive (false);
		}
	}

	public static void registerOptionsMenu(GameObject optionsMenuGO){
		if(optionsMenuGO!=null && optionsMenuO == null){
			optionsMenuO = GameObject.Instantiate (optionsMenuGO) as GameObject;
			optionsMenuO.transform.SetParent(allMenus.transform);
			optionsMenuO.SetActive (false);
		}
	}

	public static void registerBlackMenu(GameObject blackMenuGO){
		if(blackMenuGO!=null && blackMenuO == null){
			blackMenuO = GameObject.Instantiate (blackMenuGO) as GameObject;
			blackMenuO.transform.SetParent(allMenus.transform);
			blackMenuO.SetActive (false);
		}
	}

	public static void registerFadeManager(GameObject fadeManagerGO){
		fadeManager = fadeManagerGO.GetComponent<FadeManager> ();
	}

	public static void registerPlayingGUI(GameObject playingGUIGO){
		if(playingGUIGO!=null && playingGUIO == null){
			playingGUIO = GameObject.Instantiate (playingGUIGO) as GameObject;
			corruptionBar = playingGUIO.GetComponentInChildren<CorruptionProgress>().gameObject;
			minimapGUI = playingGUIO.GetComponentInChildren<MinimapGUI>().gameObject;
			corruptionBar.SetActive(false);
			playingGUIO.SetActive (false);
		}
	}

	public static void registerSpaceJumpGUI(GameObject spaceJumpGUIGO){
		if(spaceJumpGUIGO!=null && spaceJumpGUIO == null){
			spaceJumpGUIO = GameObject.Instantiate (spaceJumpGUIGO) as GameObject;
			spaceJumpGUIO.SetActive (false);
		}
	}

	public static void registerTutorialMenu(GameObject tutorialMenuGO){
		if(tutorialMenuGO!=null && tutorialMenuO == null){
			tutorialMenuO = GameObject.Instantiate (tutorialMenuGO) as GameObject;
			tutorialText = tutorialMenuO.GetComponentInChildren<Text>();
			tutorialMenuO.SetActive (false);
		}
	}

	public static void registerOnRotatingPlanetMenu(GameObject rotatingPlanetMenuGO){
		if(rotatingPlanetMenuGO!=null && rotatingOnPlanetMenuO == null){
			rotatingOnPlanetMenuO = GameObject.Instantiate (rotatingPlanetMenuGO) as GameObject;
			rotatingOnPlanetMenuO.SetActive (false);
		}
	}


	public static void activatePlayingGUIWithFadeIn(){
		playingGUIO.SetActive (true);
		fadeManager.fadeInCoroutine (playingGUIO,null,2f);
	}

	public static void deactivatePlayingGUI(){
		playingGUIO.SetActive (false);
	}

	public static void activateTutorialText(){
		tutorialMenuO.SetActive (true);
	}

	public static void deactivateTutorialText(){
		tutorialMenuO.SetActive (false);
	}

	public static void setTutorialText(string text){
		tutorialText.text = text;
	}

	
	public static void activateMinimapGUI(){
		minimapGUI.SetActive (true);
	}

	public static void deactivateMinimapGUI(){
		minimapGUI.SetActive (false);
	}

	public static void activateOnRotatingPlanetMenu(){
		rotatingOnPlanetMenuO.SetActive (true);
	}

	public static void deactivateOnRotatingPlanetMenu(){
		rotatingOnPlanetMenuO.SetActive (false);
	}

	public static void deactivatePlayingGUIWithFadeOut(){
		fadeManager.fadeOutCoroutine(playingGUIO,deactivatePlayingGUI,2f);
	}

	public static void activateSpaceJumpGUI(){
		if(!isSpaceGUIActive){
			spaceJumpGUIO.SetActive (true);
			isSpaceGUIActive = true;
		}
	}

	public static void setPercentageOfBreathing(float percentage){
		if(spaceJumpGUIO!=null){
			BreathingGUI breathingGUI = spaceJumpGUIO.GetComponentInChildren<BreathingGUI> ();
			if(breathingGUI!=null){
				spaceJumpGUIO.GetComponentInChildren<BreathingGUI> ().setPercentage (percentage);
			}
		}
	}
	
	public static void deactivateSpaceJumpGUI(){
		if(isSpaceGUIActive){
			spaceJumpGUIO.SetActive (false);
			isSpaceGUIActive = false;
		}
	}

	public static void activateMenu(Menu newMenu){
		deactivateMenus ();
		GameObject newMenuGO = getMenu (newMenu);
		if(newMenuGO!=null){
			getMenu (newMenu).SetActive (true);
		}
		actualMenu = getMenu (newMenu);
		if(actualMenu!=null){
			MenuManager man = actualMenu.GetComponent<MenuManager>();
			if(man!=null){
				EventSystem.current.SetSelectedGameObject (man.firstButtonFocus);
			}
		}

		actualMenuActivated = newMenu;
	}



	public static void openCraftingMenu (){
		if(actualMenuActivated.Equals(Menu.None)){
			activateMenu(Menu.CraftingMenu);
			GameManager.pauseGame();
		}
	}

	public static void closeCraftingMenu(){
		activateMenu (Menu.None);
		GameManager.unPauseGame ();
	}

	public static void activatePauseMenu(){
		fadeIn (Menu.OnPauseMenu);
	}

	public static void deactivatePauseMenu(){
		activateMenu (Menu.None);
	}

	public static void activateCorruptionBar(){
		corruptionBar.SetActive (true);
		corruptionBar.GetComponent<CanvasGroup> ().alpha = 1f;
	}

	public static void deactivateCorruptionBarC(){
		corruptionBar.SetActive (false);
	}

	public static void deactivateCorruptionBar(){
		fadeManager.fadeOutWithSpeed (deactivateCorruptionBarC, corruptionBar,2f);
	}

	public static void setPercentageCorruption(float percentage){
		corruptionBar.GetComponent<CorruptionProgress> ().setPercentage (percentage);
	}

	public static Vector3 getCorruptionBarPosition(){
		Ray ray = GameManager.mainCamera.GetComponent<Camera> ().ScreenPointToRay (corruptionBar.GetComponent<CorruptionProgress> ().getPixelPositionCorruptionBar ());
		Plane plane = new Plane (Vector3.forward * -1f, Vector3.forward * -(GameManager.player.transform.position.z));
		//Ray ray = new Ray (position, GameManager.mainCamera.transform.forward);
		float distance;
		if(plane.Raycast (ray, out distance)){
	
			return ray.GetPoint (distance);
		}else{
			return new Vector3(0f,0f,0f);
		}
	}

	public static bool showLeafsInActualMenu(){
		if(actualMenu!=null && actualMenu != introSceneO){		
			if(actualMenu.GetComponent<MenuManager>().menuWithLeaves && actualMenu.GetComponent<CanvasGroup>().alpha == 1f){
				return true;
			}
		}
		return false;
	}

	public static void activateInteractuablePopup(){
		if(actualMenuActivated.Equals(Menu.None)){
			interactuablePopupO.SetActive (true);
		}
	}

	public static void deactivateInteractuablePopup(){
		interactuablePopupO.SetActive (false);
	}


	
	public static void fadeOutChangeMenuFadeIn(Menu newMenu){
		nextMenu = newMenu;
		fadeManager.fadeOut(changeMenuAndFadeIn,actualMenu);
	}

	public static void fadeOutChangeMenuFadeInWithAction(Action action,Menu newMenu){
		nextMenu = newMenu;
		fadeManager.fadeOut(changeMenuAndFadeInWithAction,actualMenu);
		actionToDo = action;
	}

	public static void fadeInWithAction(Action action,Menu newMenu){
		activateMenu (newMenu);
		fadeManager.fadeInWithAction(action, getMenu(newMenu));
	}

	public static void fadeIn(Menu newMenu){
		activateMenu (newMenu);
		fadeManager.fadeIn (getMenu (newMenu));

	}

	public static void fadeOut(Action action,Menu newMenu){
		fadeManager.fadeOut (action, getMenu (newMenu));
		actualMenuActivated = Menu.None;
	}

	public static void fadeOut(Action action){
		fadeManager.fadeOut (action, actualMenu);
		actualMenuActivated = Menu.None;
	}

	public static void fadeAllOut(Action action){
		fadeManager.fadeAllOut (action);
		actualMenuActivated = Menu.None;
	}

	public static void fadeAllIn(){
		fadeManager.fadeAllIn ();
	}

	public static void getHurtEffect(){
		fadeManager.getHurtEffect ();
	}

	public static void registerHightlightFollower(GameObject highlightGO){
		if(highlightFollowers==null){
			highlightFollowers = new List<FollowHighlightedButton>(0);
		}
		highlightFollowers.Add (highlightGO.GetComponent<FollowHighlightedButton> ());
	}

	public static void informHighlighted(GameObject objectHighlighted){
		foreach(FollowHighlightedButton highlightFollower in highlightFollowers){
			highlightFollower.informHighlightedObject(objectHighlighted);
		}
	}

	public static void informUnhighlighted(GameObject objectHighlighted){
		foreach(FollowHighlightedButton highlightFollower in highlightFollowers){
			highlightFollower.informUnhighlightedObject(objectHighlighted);
		}
	}
}
