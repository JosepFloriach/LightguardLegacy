using UnityEngine;
using System.Collections;

public class PersistentData{
		
	public int playerLastCheckpoint;

	public bool spaceJumpUnlocked;

	private void initializeGameState(){
		playerLastCheckpoint = 0;
		spaceJumpUnlocked = false;

	}

	public PersistentData(){
		load ();
		//initializeGameState ();
	}

	public void save(){
		PlayerPrefs.SetInt ("playerLastCheckpoint", playerLastCheckpoint);
		saveBool ("spaceJumpUnlocked", spaceJumpUnlocked);
		PlayerPrefs.Save ();
	}

	private void saveBool(string name,bool value){
		int boolvalue = 0;
		if (value) { boolvalue = 1;}
		PlayerPrefs.SetInt (name, boolvalue);
	}

	private bool readBool(string name){
		int boolValue = PlayerPrefs.GetInt(name);
		if(boolValue == 0){
			return false;
		}else{
			return true;
		}
	}

	public void load(){
		if (GameManager.loadAndSave) {
			if (PlayerPrefs.HasKey ("playerLastCheckpoint")) {
				playerLastCheckpoint = PlayerPrefs.GetInt ("playerLastCheckpoint");
				spaceJumpUnlocked = readBool ("spaceJumpUnlocked");
			} else {
				initializeGameState ();
			}
		} else {
			PlayerPrefs.DeleteAll();
			initializeGameState();
		}
	}
}
