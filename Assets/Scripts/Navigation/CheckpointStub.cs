using UnityEngine;
using System.Collections;

public class CheckpointStub{
	public GameObject checkPointObject;

	public int checkPointIndex;

	public CheckpointStub(GameObject go,int index){
		checkPointObject = go;
		checkPointIndex = index;
	}
}
