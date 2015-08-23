using UnityEngine;
using System.Collections;

public class PlanetCorrupted : Planet {
	private PlanetSpawnerManager planetSpawnerManager;
	private PlanetCorruption planetCorruption;
	private PlanetEventsManager planetEventsManager;

	protected override void initialize(){
		planetSpawnerManager = GetComponent<PlanetSpawnerManager> ();
		planetCorruption = GetComponent<PlanetCorruption> ();
		planetEventsManager = GetComponent<PlanetEventsManager> ();
	}

	public override bool isPlanetCorrupted(){
		return true;
	}

	public bool canPlayerSpaceJumpInPlanet(){
		if(planetCorruption!=null && planetCorruption.isCorrupted()){
			return false;
		}
		return true;
	}

	protected override void virtualActivate(){
		if(planetCorruption!=null && planetCorruption.isCorrupted()){
			planetCorruption.activateSpawning();
		}
		if(planetEventsManager!=null){
			planetEventsManager.activate();
		}
	}

	protected override void virtualDeactivate(){
		if(planetSpawnerManager!=null){
			planetSpawnerManager.deactivate();
		}
		if(planetEventsManager!=null){
			planetEventsManager.deactivate();
		}
	}
	
	public PlanetSpawnerManager getPlanetSpawnerManager(){
		return planetSpawnerManager;
	}

	public PlanetCorruption getPlanetCorruption(){
		return planetCorruption;
	}

	public PlanetEventsManager getPlanetEventsManager(){
		return planetEventsManager;
	}
}
