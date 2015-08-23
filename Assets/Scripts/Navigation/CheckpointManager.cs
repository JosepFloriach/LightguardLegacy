using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour {

	List<CheckpointStub> checkPointsList = new List<CheckpointStub> (0);

	void Awake(){
		GameManager.registerCheckpointManager (gameObject);
	}
	
	public void registerCheckpoint(GameObject checkpoint,int index){
		CheckpointStub stub = new CheckpointStub (checkpoint, index);
		checkPointsList.Add (stub);
	}

	public CheckpointStub getLastCheckpoint(){
		for(int i = 0;i<checkPointsList.Count;++i){
			if(checkPointsList[i].checkPointIndex == GameManager.persistentData.playerLastCheckpoint){
				return checkPointsList[i];
			}
		}
		return null;
	}

	public CheckpointStub getCheckpointByIndex(int index){
		for(int i = 0;i<checkPointsList.Count;++i){
			if(checkPointsList[i].checkPointIndex == index){
				return checkPointsList[i];
			}
		}
		return null;
	}

}
