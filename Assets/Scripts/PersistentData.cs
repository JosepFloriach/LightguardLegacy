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
		initializeGameState ();
	}
}
