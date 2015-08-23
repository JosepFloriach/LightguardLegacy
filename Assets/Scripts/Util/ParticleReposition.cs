using UnityEngine;
using System.Collections;

public class ParticleReposition : MonoBehaviour {

	public GameObject objectToPutParticlesOn;
	ParticleSystem particleSystem;
	SkinnedMeshRenderer skin;

	public float updatesPerSecond = 4;
	private float timeToUpdate;
	private float timer;
	// Use this for initialization
	void Start () {
		timer = 0f;
		timeToUpdate = 1 / updatesPerSecond;
		particleSystem = GetComponent<ParticleSystem> ();
		skin = objectToPutParticlesOn.GetComponent<SkinnedMeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void LateUpdate(){
		timer += Time.deltaTime;
		if(timer>=timeToUpdate){
			timer = 0f;
			Mesh baked = new Mesh();
			skin.BakeMesh(baked);

			int index = 0;
			int total = 0;
			Vector3 pos = Vector3.zero;
			ParticleSystem.Particle[] gos = new ParticleSystem.Particle[particleSystem.particleCount];
			total = particleSystem.GetParticles(gos);
			
			while(index < total){
				pos = gos[index].position;
				int indexV = (int)(((float)index/(float)total)*baked.vertices.Length);
				pos.x = baked.vertices[indexV%baked.vertices.Length].x;
				pos.y = baked.vertices[indexV%baked.vertices.Length].y;
				pos.z = baked.vertices[indexV%baked.vertices.Length].z;
				//pos = objectToPutParticlesOn.transform.TransformPoint(pos); 
				gos[index].position = pos;
				index++;
			}

			particleSystem.SetParticles(gos,gos.Length);
		}
	}
}
