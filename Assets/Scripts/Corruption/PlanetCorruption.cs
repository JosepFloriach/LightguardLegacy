using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlanetCorruption : MonoBehaviour {

	public GameObject shintoDoor;
	public GameObject corruptionOrigin;
	public GameObject corruptionParticles;
	private ShintoDoor shinto;
	public Material material;
	public float yMin = -3f;
	public float yMax = 30f;
	public bool startCorrupted = true;

	public float speed = 5f;
	public float particlesOnPurificationSpeed = 5f;
	private float direction = -1f;
	private float yValue = 0f;
	private float offset = 0f;
	private bool cleaningCorruption = false;

	private List<Material> materials;
	private Vector4 originPosition;
	private bool spawningEnabled;
	// Use this for initialization
	void Awake () {
		yValue = yMin;
	}

	void Start(){
		if(shintoDoor!=null){
			shinto = shintoDoor.GetComponent<ShintoDoor> ();
		}
		originPosition = new Vector4 (corruptionOrigin.transform.position.x, corruptionOrigin.transform.position.y, corruptionOrigin.transform.position.z, 0f);
		ParticleSystem particlesOnDescorrupt = corruptionOrigin.GetComponent<ParticleSystem> ();
		if(particlesOnDescorrupt!=null){
			particlesOnDescorrupt.startSpeed = particlesOnPurificationSpeed;
			particlesOnDescorrupt.startLifetime = ((float)(yMax-yMin))/Mathf.Abs (particlesOnPurificationSpeed);
		}
		if(!startCorrupted){
			setCorruptionToClean();
			if(GetComponent<PlanetSpawnerManager>()!=null){
				GetComponent<PlanetSpawnerManager> ().deactivate();
			}
		}
	}

	public void initialize(){
		materials = new List<Material> (0);
		foreach(Renderer renderer in GetComponentsInChildren<Renderer>(true)){
				foreach(Material material in renderer.materials){
					if(material.HasProperty("_YCutOut")){
						materials.Add(material);
					}
				}
		}

	}

	//It instantly sets the planet to it's clean state
	public void setCorruptionToClean(){
		yValue = yMax;
		direction = 1f;
		spawningEnabled = false;
		GUIManager.deactivateCorruptionBarC ();
		GUIManager.activateMinimapGUI();

		if(GameManager.playerSpaceBody.getClosestPlanet()!=null){
			if(GameManager.playerSpaceBody.getClosestPlanet().isPlanetCorrupted()){
				(GameManager.playerSpaceBody.getClosestPlanet() as PlanetCorrupted).getPlanetEventsManager().planetCleansed();
			}
		}

	}

	public void corrupt(){
		corruptionParticles.GetComponent<ParticleSystem> ().Play ();
		direction = -1f;
	}

	public void activateSpawning(){
		spawningEnabled = true;
		if(GetComponent<PlanetSpawnerManager>()!=null){
			GetComponent<PlanetSpawnerManager> ().activate ();
		}
	}


	//It plays the effect of corruption being cleansed
	public void cleanCorruption(){
		corruptionParticles.GetComponent<ParticleSystem> ().Stop();
		if(!cleaningCorruption){
			cleaningCorruption = true;
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().setObjectiveZCameraCleansePlanet ();
			corruptionOrigin.transform.up = corruptionOrigin.transform.position - transform.position;
			GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().followObjective(corruptionOrigin,13f,2.5f);
			GameManager.audioManager.playSong(5);
			StartCoroutine("startCleaningWithDelay");
		}
	}

	private IEnumerator startCleaningWithDelay(){
		GameManager.inputController.disableInputController ();
		GUIManager.deactivatePlayingGUIWithFadeOut ();
		float timer = 0f;
		while(timer<2f){
			timer+=Time.deltaTime;
			yield return null;
		}
		direction=1f;
		shinto.activateKanjis ();
		corruptionOrigin.GetComponent<ParticleSystem> ().Play ();
		
	}

	public void isFinishedCleaning(){
		GameManager.inputController.enableInputController ();
		GUIManager.activatePlayingGUIWithFadeIn ();
		cleaningCorruption = false;
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().returnOriginalZ ();
		GameManager.mainCamera.GetComponent<CameraFollowingPlayer> ().resetObjective ();
		GUIManager.activateMinimapGUI();
		if(GameManager.playerSpaceBody.getClosestPlanet().isPlanetCorrupted()){
			PlanetEventsManager pem = (GameManager.playerSpaceBody.getClosestPlanet() as PlanetCorrupted).getPlanetEventsManager();
			if(pem!=null){
				pem.planetCleansed();
			}
		}
		GameManager.persistentData.spaceJumpUnlocked = true;
		GameManager.audioManager.playSong(1);
	}
	// Update is called once per frame
	void Update () {
		offset += Time.deltaTime/100f;
		yValue += (direction * speed * Time.deltaTime);
		if(yValue>yMax){
			if(cleaningCorruption){
				isFinishedCleaning();
			}
			yValue = yMax;
		}
		if(yValue<yMin){
			yValue = yMin;
		}
		
			foreach(Material material in materials){
				if(material.HasProperty("_YCutOut")){
					material.SetFloat("_YCutOut",yValue);
					material.SetTextureOffset("_CorruptionTexture",new Vector2(offset,0));
					material.SetVector("_OriginCorruption",originPosition);
				}
			}

		Renderer athmosphereRenderer = GetComponent<GravityAttractor> ().getAthmosphere ().GetComponent<Renderer> ();
		foreach(Material material in athmosphereRenderer.materials){
			if(material.HasProperty("_YCutOut")){
				material.SetFloat("_YCutOut",yValue);
				material.SetVector("_OriginCorruption",originPosition);
			}
		}
	}

	public bool getSpawningEnabled(){
		return spawningEnabled;
	}
	public bool isCorrupted(){
		if(direction>=1f){
			return false;
		}else{
			return true;
		}
	}
}
