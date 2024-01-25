using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Coordinate5
{
	public int x { get; set; }
	public int y { get; set; }
	public int z { get; set; }
}


public class GreedyMeshAllAxis : MonoBehaviour
{
	public GameObject mesh;

	private int cubesPerXAxis;
	private int cubesPerYAxis;
	private int cubesPerZAxis;

	Dictionary<Vector3Int, Vector3Int> regionPos = new Dictionary<Vector3Int, Vector3Int>();
	Dictionary<Vector3Int, Vector3Int> posData = new Dictionary<Vector3Int, Vector3Int>();
	List<Coordinate5> DataItems = new List<Coordinate5>();
	List<Coordinate5> DataItemsCopy = new List<Coordinate5>();


	//Sphere stuff
	public GameObject sphere;
	SphereCollider sphereCollider;

	//Voxel stuff
	int VOXEL_SIZE = 1;

	//Anonymouse function using delegate
	public delegate Vector3Int PrintDel(Vector3Int vec); 
	public StarterAssets.StarterAssetsInputs starterAssetsInputs;

	void Start()
	{
		// Need to round to nearest whole number for odd sized parts?
		cubesPerXAxis = (int)this.transform.localScale.x;
		cubesPerYAxis = (int)this.transform.localScale.y; // + 1 // because it cant start at 0 it has to check voxel behind it at each step to see if can merge
		cubesPerZAxis = (int)this.transform.localScale.z; // + 1
		sphere = GameObject.Find("Sphere");
		sphereCollider = sphere.GetComponent<SphereCollider>();
	}

	// run algorithm anytime I want on the parts colliding with sphere
	void test(Vector3 center, float radius)
	{
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		foreach (var hitCollider in hitColliders)
		{
			if (hitCollider.name == "Sphere") continue;
			if (this.gameObject.CompareTag("Box")) CreateCube("Box", this.gameObject);
		}
	}


	// divides the size of the part by VOXEL_SIZE
	Vector3Int CalculateBlockCount(Vector3Int vec)
	{
		
		var calcVec = new Vector3Int(
			(int)Mathf.Floor(vec.x / VOXEL_SIZE),
			(int)Mathf.Floor(vec.y / VOXEL_SIZE),
			(int)Mathf.Floor(vec.z / VOXEL_SIZE)
		);
		print("check this man " + calcVec);
		return calcVec;

	}

	// CalculateBlockCount helper function
	Vector3Int CalculateBlockCountHelper(Vector3Int vec, PrintDel op)
	{
		return op(vec);
	}

	//Assign cube data ////////////////////////////////////////////////////////////////////////////////////
	// Createcube fires twice if the sphere colldies with the ground (fix this in future)
	void CreateCube(string tag, GameObject targetPart)
	{
		print("create cube " + targetPart);
		Vector3Int blockCount = CalculateBlockCountHelper(new Vector3Int(
		(int)targetPart.transform.localScale.x,
		(int)targetPart.transform.localScale.y,
		(int)targetPart.transform.localScale.z),
		CalculateBlockCount
		);

		Vector3Int blockSize = new Vector3Int(
		(int)targetPart.transform.localScale.x / blockCount.x,
		(int)targetPart.transform.localScale.y / blockCount.y,
		(int)targetPart.transform.localScale.z / blockCount.z
		);

		print("blockCount " + blockCount);
		print("blockSize " + blockSize);

		for (int x = 0; x < cubesPerXAxis; x++)
		{
			for (int y = 0; y < cubesPerYAxis; y++)
			{
				for (int z = 0; z < cubesPerZAxis; z++)
				{
					Vector3Int offset = new Vector3Int(
						(int)targetPart.transform.localScale.x,  
						(int)targetPart.transform.localScale.y,
						(int)targetPart.transform.localScale.z 
					) + blockSize / 2;

					Vector3 vecTest = targetPart.transform.TransformDirection(new Vector3( 
						x * blockSize.x,
						y * blockSize.y,
						z * blockSize.z)) - offset;

					//(x * blockSize.x) - offset.x, //+ 0.5f
					//(y * blockSize.y) - offset.y, //+ 4
					//(z * blockSize.z) - offset.z) //+ 0.5f


					print("check offset " + vecTest);

					//Get the magnitude between the block and the sphere
					var distance = (sphereCollider.transform.position - vecTest).magnitude;
					if (distance > (sphereCollider.transform.localScale.y / 2))
					{
						posData[new Vector3Int(x, y, z)] = new Vector3Int((int)vecTest.x, (int)vecTest.y, (int)vecTest.z);
						regionPos[new Vector3Int(x, y, z)] = new Vector3Int(1, 1, 1);
						print("part is within the sphere zero " + distance);

						// Instantiate cube
						
					}
					else
					{
						print("part is within the sphere " + distance);
						//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						//cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
						//cube.tag = "Box";
						//cube.transform.localScale = new Vector3(
						//	1,
						//	1,
						//	1
						//);
						//cube.transform.position = vecTest;
					}
				}
			}
		}

		Vector3Int myValue;

		// Z meshing
		foreach (Vector3Int key in regionPos.Keys)
		{
			if (!regionPos.TryGetValue(new Vector3Int(key.x, key.y, key.z), out myValue))
			{
				continue;
			}

			if (!regionPos.TryGetValue(new Vector3Int(key.x, key.y, key.z - 1), out myValue))
			{
				continue;
			}

			DataItems.Add(new Coordinate5
			{
				x = key.x,
				y = key.y,
				z = key.z,
			});
		}
		int counter = -1;

		foreach (var data in DataItems)
		{
			counter++;
			if (counter < 1) continue;

			if (!regionPos.TryGetValue(new Vector3Int(data.x, data.y, data.z), out myValue))
			{
				continue;
			}

			if (!regionPos.TryGetValue(new Vector3Int(data.x, data.y, data.z - 1), out myValue))
			{
				continue;
			}

			regionPos[new Vector3Int(data.x, data.y, data.z)] += new Vector3Int(0, 0, data.z - 1);
			regionPos.Remove(new Vector3Int(data.x, data.y, data.z - 1));
			var prevData = new Vector3Int(data.x, data.y, data.z - 1);

			//DataItemsCopy.Add(new Coordinate5
			//{
			//	x = data.x,
			//	y = data.y,
			//	z = data.z - 1,
			//});
		}

		// Remove z from data items so we don't iterate over them in y mehsing
		//foreach (var data in DataItemsCopy)
		//      {

		//	bool haha = DataItems.Remove(new Coordinate5
		//	{
		//		x = data.x,
		//		y = data.y,
		//		z = data.z,
		//	});

		//	print("remove data " + haha);
		//}

		// Y meshing
		 counter = -1;
		foreach (var data in DataItems)
		{
			counter++;
			if (counter < 1) continue;
			if (!regionPos.TryGetValue(new Vector3Int(data.x, data.y, data.z), out myValue))
			{
				continue;
			}


			if (!regionPos.TryGetValue(new Vector3Int(data.x, data.y - 1, data.z), out myValue))
			{
				continue;
			}

			if (data.z != regionPos[new Vector3Int(data.x, data.y - 1, data.z)].z) continue;

            regionPos[new Vector3Int(data.x, data.y, data.z)] += new Vector3Int(0, data.y - 1, 0);
            regionPos.Remove(new Vector3Int(data.x, data.y - 1, data.z));
        }


		//Create meshes
		// Issue creating cubes in same place?
		// then sort out offset
		// then sort out y meshing
		//counter = -1;
		//counter++;
		//if (counter < 2) continue;
		foreach (Vector3Int key in regionPos.Keys)
		{
			
			//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//print("cube size x " + regionPos[new Vector3Int(key.x, key.y, key.z)].x + " /y " + regionPos[new Vector3Int(key.x, key.y, key.z)].y + " /z " + regionPos[new Vector3Int(key.x, key.y, key.z)].z);
			//cube.transform.localScale = new Vector3(
			//	regionPos[new Vector3Int(key.x, key.y, key.z)].x, //size.Value["X"]
			//	regionPos[new Vector3Int(key.x, key.y, key.z)].y,
			//	regionPos[new Vector3Int(key.x, key.y, key.z)].z  //added this to get right size should be easy fix though
			//);

			//var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

			//var calc = new Vector3(
			//		 key.x - regionPos[new Vector3Int(key.x, key.y, key.z)].x + cube.transform.localScale.x,
			//		(key.y - regionPos[new Vector3Int(key.x, key.y, key.z)].y / 2) - cube.transform.localScale.y / 2,
			//		(key.z - regionPos[new Vector3Int(key.x, key.y, key.z)].z / 2) - cube.transform.localScale.z / 2
			//	);

			//var pos = test + calc;
			////var pos = vecTestt;

			//cube.tag = "Box";
			//cube.transform.position = pos;


			//Move block outta the freaking way

			///// this creates duplicates
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.name = "Cube " + counter;
			cube.tag = "Box";
			cube.GetComponent<Renderer>().material.color = Random.ColorHSV();

			cube.transform.localScale = new Vector3Int(
                         regionPos[new Vector3Int(key.x, key.y, key.z)].x, 
                         regionPos[new Vector3Int(key.x, key.y, key.z)].y, 
                         regionPos[new Vector3Int(key.x, key.y, key.z)].z 
            ) * blockSize;

			// Fixed the x axis from putting two columns in same position
			// takes the first column and places it in middle?
			//key.x - regionPos[new Vector3Int(key.x, key.y, key.z)].x + (int)cube.transform.localScale.x
			Vector3Int vecTestt = new Vector3Int(
					(posData[key].x - regionPos[new Vector3Int(key.x, key.y, key.z)].x / 2) + blockSize.x / 2, //+ blockSize.x / 2) * blockSize.x,
					(posData[key].y - regionPos[new Vector3Int(key.x, key.y, key.z)].y / 2) + blockSize.y / 2,
                    (posData[key].z - regionPos[new Vector3Int(key.x, key.y, key.z)].z / 2) + blockSize.z / 2) * blockSize;

            cube.transform.position = vecTestt;

			print("positions " + cube.transform.position);
		}

		//Move block outta the freaking way
		this.gameObject.transform.position = new Vector3Int(9999, 9999, 9999);
	}

	public void Update()
	{
		if (starterAssetsInputs.explode)
		{
			starterAssetsInputs.explode = false;
			test(sphere.transform.position, sphereCollider.transform.localScale.y / 2);
		}
	}
}


//Bets working version so far slightly off still
//Create meshes
//foreach (Vector3Int key in regionPos.Keys)
//{
//	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

//	cube.transform.localScale = new Vector3(
//		regionPos[new Vector3Int(key.x, key.y, key.z)].x, 
//		regionPos[new Vector3Int(key.x, key.y, key.z)].y,
//		regionPos[new Vector3Int(key.x, key.y, key.z)].z 
//	);

//	var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

//	var calc = new Vector3(
//			key.x - regionPos[new Vector3Int(key.x, key.y, key.z)].x + cube.transform.localScale.x,
//			(key.y - regionPos[new Vector3Int(key.x, key.y, key.z)].y / 2) - cube.transform.localScale.y / 2,
//			(key.z - regionPos[new Vector3Int(key.x, key.y, key.z)].z / 2) - cube.transform.localScale.z / 2
//		);

//	var pos = test + calc;

//	cube.tag = "Box";
//	cube.transform.position = pos;
//}

