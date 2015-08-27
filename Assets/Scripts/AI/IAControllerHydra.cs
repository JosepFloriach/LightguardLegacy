using UnityEngine;
using System.Collections;

public class IAControllerHydra : IAController {


	protected override void UpdateAI(){
		GetComponentInChildren<Animator>().SetBool ("isHidding", false);
		characterController.LookLeftOrRight (-1f);
	}


}
