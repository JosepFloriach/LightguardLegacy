using UnityEngine;
using System.Collections;

public class WeaponSmithInteraction : Interactuable {

	public override void doInteractAction() { 
		GUIManager.openCraftingMenu ();
	}
}
