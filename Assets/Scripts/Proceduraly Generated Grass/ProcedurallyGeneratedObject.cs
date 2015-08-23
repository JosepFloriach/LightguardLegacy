using UnityEngine;
using System.Collections;

public class ProcedurallyGeneratedObject : MonoBehaviour {

	public enum Billboard{Red,Blue,Green,Black};

	public int randomSeed = 0;
	public int detailLevel = 5;
	public float detailMaximumPercentage = 0.5f;

	public GameObject objectiveMesh;
	public Texture mapTexture;
	//public GameObject planet;

	public GameObject redBillboard;
	public GameObject greenBillboard;
	public GameObject blueBillboard;

	private GameObject[] levels;
	private int lastLevelPut = 0;

	private int lastLevelActivated = -1;
	public float density = 1f;

	void Awake(){

	}

	public void setDetailPercentage(float percentage){
		percentage = percentage / detailMaximumPercentage;
		percentage = Mathf.Clamp01 (percentage);
		int lastIndexToActivate = (int) (percentage * detailLevel);
		if(lastLevelActivated!=lastIndexToActivate){
			lastLevelActivated = lastIndexToActivate;
			for(int i = 0;i<levels.Length;i++){
				if(i>lastIndexToActivate){
					levels[i].SetActive(true);
				}else{
					levels[i].SetActive(false);
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		GameManager.registerProceduralGrass (this);
		if (detailLevel < 1) {
			detailLevel = 1;
		}
		levels = new GameObject[detailLevel];
		for(int i = 0;i<detailLevel;i++){
			levels[i] = new GameObject();
			levels[i].name = "Level "+i;
			levels[i].transform.parent = transform;
		}

	
		Vector2[] uvs = objectiveMesh.GetComponent<MeshFilter> ().mesh.uv;
		Vector3[] vertices = objectiveMesh.GetComponent<MeshFilter> ().mesh.vertices;
		Vector3[] normals = objectiveMesh.GetComponent<MeshFilter> ().mesh.normals;
		Texture2D texture = mapTexture as Texture2D;
		int[] triangles = objectiveMesh.GetComponent<MeshFilter> ().mesh.triangles;
		if(texture!=null){
			transform.position = objectiveMesh.transform.position;

			for(int i = triangles.Length-3;i>=0;i-=3){
				Color vertex1Col = texture.GetPixelBilinear(uvs[triangles[i]].x,uvs[triangles[i]].y);
				Color vertex2Col = texture.GetPixelBilinear(uvs[triangles[i+1]].x,uvs[triangles[i+1]].y);
				Color vertex3Col = texture.GetPixelBilinear(uvs[triangles[i+2]].x,uvs[triangles[i+2]].y);

				Vector3 vertex1Pos = vertices[triangles[i]];
				Vector3 vertex2Pos = vertices[triangles[i+1]];
				Vector3 vertex3Pos = vertices[triangles[i+2]];

				float area = getAreaSize(vertex1Pos,vertex2Pos,vertex3Pos);

				int ammountOfPlantsToPut = (int) (area * density);
				float extraArea = (area*density) - ammountOfPlantsToPut;
				float rand = Random.value;
				if(rand<=extraArea){
					ammountOfPlantsToPut++;
				}

				Vector3 normal = (normals[triangles[i]] + normals[triangles[i+1]] + normals[triangles[i+2]]) / 3f;

				int redBillboardPlants = 0;
				int greenBillboardPlants = 0;
				int blueBillboardPlants = 0;

				putPlantsNumber(ref redBillboardPlants,ref greenBillboardPlants,ref blueBillboardPlants,getColor(vertex1Col),ammountOfPlantsToPut);
				putPlantsNumber(ref redBillboardPlants,ref greenBillboardPlants,ref blueBillboardPlants,getColor(vertex2Col),ammountOfPlantsToPut);
				putPlantsNumber(ref redBillboardPlants,ref greenBillboardPlants,ref blueBillboardPlants,getColor(vertex3Col),ammountOfPlantsToPut);

				bool differentVertexColors = false;
				if((redBillboardPlants!=0 && greenBillboardPlants!=0 )
				   || (redBillboardPlants!=0 && blueBillboardPlants!=0 )
				   || (blueBillboardPlants!=0 && greenBillboardPlants!=0 )
				   || ((blueBillboardPlants+greenBillboardPlants+redBillboardPlants)<ammountOfPlantsToPut)){
					differentVertexColors = true;
				}

				//If different vertex colors, then put plants close to vertices, otherwise, random in the triangle

					createRandomPlants(redBillboardPlants,redBillboard,vertex1Pos,vertex2Pos,vertex3Pos,normal);
					createRandomPlants(greenBillboardPlants,greenBillboard,vertex1Pos,vertex2Pos,vertex3Pos,normal);
					createRandomPlants(blueBillboardPlants,blueBillboard,vertex1Pos,vertex2Pos,vertex3Pos,normal);

			}
			transform.parent = objectiveMesh.transform;
		}
	}

	void createRandomPlants(int ammountOfPlants,GameObject billboard,Vector3 vertix1,Vector3 vertix2,Vector3 vertix3,Vector3 normal){
		for(int i = 0;i<ammountOfPlants;i++){
			float r1 = Random.value;
			float r2 = Random.value;
			Vector3 randomPosition = (1f - Mathf.Sqrt(r1)) * vertix1 + (Mathf.Sqrt(r1) * (1 - r2)) * vertix2 + (Mathf.Sqrt(r1) * r2) * vertix3;
			createNewBillboard(billboard,randomPosition,normal);
		}

	}

	void putPlantsNumber(ref int redPlants,ref int greenPlants,ref int bluePlants,Billboard billboard,int ammountOfTotalPlants){
		float toAdd = (ammountOfTotalPlants * 0.33f);
		int intPart = (int) toAdd;
		float floatPart = toAdd - intPart;
		if(Random.value<=floatPart){
			intPart++;
		}
		if(billboard.Equals(Billboard.Blue)){
			bluePlants+=(int) intPart;
		}else if(billboard.Equals(Billboard.Red)){
			redPlants+=(int) intPart;
		}else if(billboard.Equals(Billboard.Green)){
			greenPlants+=(int) intPart;
		}
	}


	Billboard getColor(Color vertexColor){
		if(vertexColor.r > vertexColor.b &&vertexColor.r>vertexColor.g){
			return Billboard.Red;
		}else if(vertexColor.g > vertexColor.b &&vertexColor.g>vertexColor.r){
			return Billboard.Green;
		}else if(vertexColor.b > vertexColor.r &&vertexColor.b>vertexColor.g){
			return Billboard.Blue;
		}
		return Billboard.Black;
	}

	float getAreaSize(Vector3 p1,Vector3 p2,Vector3 p3){
		Vector3 A = p1 * objectiveMesh.transform.lossyScale.x;
		Vector3 B = p2 * objectiveMesh.transform.lossyScale.x;
		Vector3 C = p3 * objectiveMesh.transform.lossyScale.x;

		Vector3 V = Vector3.Cross(A-B, A-C);
		float area = V.magnitude * 0.5f;
		area = area;
		return area;
	}

	void createNewBillboard(GameObject billboard,Vector3 position,Vector3 normal){
		GameObject newObject = Instantiate(billboard) as GameObject;
		position = objectiveMesh.transform.rotation * position;
		newObject.transform.position = (objectiveMesh.transform.position+(position * objectiveMesh.transform.lossyScale.y ));

		newObject.transform.parent = levels[lastLevelPut].transform;
		lastLevelPut = (lastLevelPut + 1) % detailLevel;
		newObject.transform.rotation = Quaternion.LookRotation (Vector3.forward, position);
		//newObject.transform.up = (normal).normalized;
		newObject.transform.position += newObject.transform.up * (billboard.GetComponent<MeshRenderer> ().bounds.size.y * 0.5f);
		
		//float zrotation = newObject.transform.eulerAngles.z;
		//newObject.transform.forward = Vector3.forward;
		newObject.transform.rotation = Quaternion.LookRotation (Vector3.forward, position);
		//newObject.transform.eulerAngles = new Vector3 (newObject.transform.eulerAngles.x, newObject.transform.eulerAngles.y, zrotation);
		//newObject.transform.Rotate (billboard.transform.eulerAngles);

	}

}
