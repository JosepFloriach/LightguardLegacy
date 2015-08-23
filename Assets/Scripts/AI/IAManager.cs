using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAManager : MonoBehaviour {

	//List of the actual IA's
	List<IAController> actualIAs;

	void Awake(){
		GameManager.registerIAManager (this);
		actualIAs = new List<IAController> (0);
	}

	void Update(){

	}

	public void registerIA(IAController iaC){
		actualIAs.Add (iaC);
	}

	public void removeIA(IAController iaC){
		actualIAs.Remove (iaC);
		iaC.gameObject.SetActive (false);
		Destroy (iaC.gameObject);
	}

	public void disableIAs(){
		foreach(IAController iaController in actualIAs){
			iaController.deactivate();
		}
	}

	public void enableIAs(){
		foreach(IAController iaController in actualIAs){
			iaController.activate();
		}
	}

	public List<IAController> getActualAIs(){
		return actualIAs;
	}
}
