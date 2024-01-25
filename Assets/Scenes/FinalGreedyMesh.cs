using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Voxel
{
	public Vector3 pos { get; set; }
	//public Vector3 size { get; set; }
	//public Color color { get; set; }
}


public class FinalGreedyMesh : MonoBehaviour
{
	Dictionary<Vector3, Vector3> regionPos = new Dictionary<Vector3, Vector3>();
	Dictionary<Vector3, Vector3> posData = new Dictionary<Vector3, Vector3>();
	List<Voxel>VoxelData = new List<Voxel>();

	//Sphere stuff
	public GameObject sphere;
	SphereCollider sphereCollider;

	//Voxel stuff
	int VOXEL_SIZE = 1;

	//Anonymouse function using delegate
	public delegate Vector3 PrintDel(Vector3 vec);
	public StarterAssets.StarterAssetsInputs starterAssetsInputs;

	void Start()
	{
		sphere = GameObject.Find("Sphere");
		sphereCollider = sphere.GetComponent<SphereCollider>();
	}

	//Run algorithm on every cube that collides with sphere
	void test(Vector3 center, float radius)
	{
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		foreach (var hitCollider in hitColliders)
		{
			if (hitCollider.name == "Sphere") continue;
			if (this.gameObject.CompareTag("Box")) CreateCube("Box", this.gameObject);
		}
	}


	//Divides the size of the part by VOXEL_SIZE
	Vector3 CalculateBlockCount(Vector3 vec)
	{
		var calcVec = new Vector3(
			Mathf.Floor(vec.x / VOXEL_SIZE),
			Mathf.Floor(vec.y / VOXEL_SIZE),
			Mathf.Floor(vec.z / VOXEL_SIZE)
		);
		return calcVec;
	}

	//CalculateBlockCount helper function
	Vector3 CalculateBlockCountHelper(Vector3 vec, PrintDel op)
	{
		return op(vec);
	}

	//Assign cube data 
	void CreateCube(string tag, GameObject target)
	{
		print("create cube " + target);
		Vector3 blockCount = CalculateBlockCountHelper(new Vector3(
		target.transform.localScale.x,
		target.transform.localScale.y,
		target.transform.localScale.z),
		CalculateBlockCount
		);

		Vector3 blockSize = new Vector3(
		target.transform.localScale.x / blockCount.x,
		target.transform.localScale.y / blockCount.y,
		target.transform.localScale.z / blockCount.z
		);

		for (int x = 0; x < blockCount.x; x++)
		{
			for (int y = 0; y < blockCount.y; y++)
			{
				for (int z = 0; z < blockCount.z; z++)
				{
					Vector3 offset = (target.transform.localScale + blockSize) / 2;
					Vector3 toWorldSpace = target.transform.TransformDirection(Vector3.Scale(new Vector3(x, y, z), blockSize) - offset);
					Vector3 pos = (target.transform.position + toWorldSpace) + new Vector3(1, 1, 1); 

                    //Get the magnitude between the block and the sphere
                    var distance = (sphereCollider.transform.position - pos).magnitude; //(test + calc)
					if (distance > (sphereCollider.transform.localScale.y / 2))
					{
						posData[new Vector3(x, y, z)] = pos;
						regionPos[new Vector3(x, y, z)] = new Vector3(1, 1, 1);
						print("Test THIS " + z);
					}
					else
                    {
						GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
						cube.GetComponent<Renderer>().material.color = Color.red;
						cube.tag = "Box";
						cube.transform.localScale = new Vector3(
							1,
							1,
							1
						);

						cube.transform.position = pos;
					}
				}
			}
		}

		Vector3 myValue;

		// Z merging
		//foreach (Vector3 key in regionPos.Keys) 
		//{
		//	VoxelData.Add(new Voxel
		//	{
		//		pos = key
		//	}); 
		//}
		//List<Vector3> keysCopy = new List<Vector3>(regionPos.Keys);
		//List<Vector3> keysToRemove = new List<Vector3>();
		//// It could be because since I cant remove while iterating over it that it still loops over voxels that have been removed?
		//foreach (var data in keysCopy)
		//{
		//	if (!regionPos.TryGetValue(data, out myValue)) continue;
		//	if (!regionPos.TryGetValue(new Vector3(data.x, data.y, -(data.z - 1)), out myValue)) continue;

		//	regionPos[data] += new Vector3(0, 0, -(data.z - 1));
		//	keysToRemove.Add(new Vector3(data.x, data.y, -(data.z - 1)));
		//	print("why " + (data.z - 1) + 1);
		//}

		//// Remove keys after iteration
		//foreach (var key in keysToRemove)
		//{
		//	regionPos.Remove(key);
		//}

		//      List<Vector3> keysCopy = new List<Vector3>(regionPos.Keys);

		//      foreach (var data in keysCopy)
		//{
		//          if (!regionPos.TryGetValue(data, out myValue)) continue;
		//          //if (!regionPos.TryGetValue(new Vector3(data.x, data.y, data.z - 1), out myValue)) continue;
		//          if (!regionPos.TryGetValue(new Vector3(data.x, data.y, -(data.z - 1)), out myValue)) continue;

		//          //regionPos[data] += new Vector3(0, 0, (data.z - 1) + 1);
		//          //regionPos.Remove(new Vector3(data.x, data.y, (data.z - 1)));

		//          regionPos[data] += new Vector3(0, 0, -(data.z - 1));
		//          regionPos.Remove(new Vector3(data.x, data.y, -(data.z - 1)));
		//          print("why " + (data.z - 1) + 1);
		//      }

		//List<Vector3> keysCopy = new List<Vector3>(regionPos.Keys);

		//foreach (var data in keysCopy) // Use ToArray to create a copy
		//{
		//	if (!regionPos.TryGetValue(data, out myValue)) continue;

		//	Vector3 newDataKey = new Vector3(data.x, data.y, (data.z - 1));

		//	if (!regionPos.TryGetValue(newDataKey, out myValue)) continue;

		//	regionPos[data] += new Vector3(0, 0, (data.z - 1) + 1); 
		//	regionPos.Remove(new Vector3(data.x, data.y, (data.z - 1)));
		//	print("why " + regionPos[data].z);
		//}
			
		// Maybe try fix indexing for positive first?

		List<Vector3> keysCopy = new List<Vector3>(regionPos.Keys);
		int someCounter = 0;
		foreach (var data in keysCopy)
		{
			print("data check " + data);
			if (!regionPos.TryGetValue(data, out myValue)) continue; 

			Vector3 newDataKey = new Vector3(data.x, data.y, (data.z - 1));

			if (!regionPos.TryGetValue(newDataKey, out myValue)) continue;

			//if ((data.z - 1) <= 0) continue;
			//if (regionPos[new Vector3(data.x, data.y, data.z - 1)].z <= 0) print("continue brother ");  continue;

			Vector3 modifiedValue;
			//if ((data.z) >= 1)  //someCounter == 1 || someCounter == 4

			//{
			//	print("check one and four " + someCounter + " / " + (data.z - 1));
			//	modifiedValue = new Vector3(0, 0, (data.z - 1) + 1);
			//}
			//else
   //         {
				modifiedValue = new Vector3(0, 0, (data.z - 1));
		//	}


			regionPos[data] += modifiedValue;
			regionPos.Remove(new Vector3(data.x, data.y, (data.z - 1)));

			print($"Modified {data}: Old Value = {regionPos[data] - modifiedValue}, New Value = {regionPos[data]}" + someCounter);
			someCounter++;
		}


		//Create meshes
		int counter = -1;
		foreach (Vector3 key in regionPos.Keys)	
		{
			counter++;
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.name = "Cube " + counter;
			cube.tag = "Box";
			cube.GetComponent<Renderer>().material.color = Random.ColorHSV(); //this.transform.GetComponent<Renderer>().material.color;

			var calc = Vector3.Scale(new Vector3(
			regionPos[new Vector3(key.x, key.y, key.z)].x,
			regionPos[new Vector3(key.x, key.y, key.z)].y,
			regionPos[new Vector3(key.x, key.y, key.z)].z), blockSize);

			cube.transform.localScale = Vector3.Scale(calc, blockSize);

			cube.transform.position = Vector3.Scale(new Vector3(
			((posData[key].x - regionPos[key].x / 2) + blockSize.x / 2),
			((posData[key].y - regionPos[key].y / 2) + blockSize.y / 2),
			((posData[key].z - regionPos[key].z / 2) + blockSize.z / 2)), blockSize);
		}

		//foreach (var data in VoxelData)
		//{
		//    if (!regionPos.TryGetValue(data.pos, out myValue)) continue;
		//    if (!regionPos.TryGetValue(new Vector3(data.pos.x, data.pos.y, data.pos.z - 1), out myValue)) continue;
		//    print("check key " + (data.pos.z - 1));

		//    regionPos[data.pos] += new Vector3(0, 0, (data.pos.z - 1) + 1);
		//    regionPos.Remove(new Vector3(data.pos.x, data.pos.y, (data.pos.z - 1)));

		//         //print("check value " + regionPos[data.pos]);

		//         ///////////////// below are chat gpt changes
		//         if (!regionPos.TryGetValue(data.pos, out myValue)) continue;
		//         if (!regionPos.TryGetValue(new Vector3(data.pos.x, data.pos.y, data.pos.z - 1), out myValue)) continue;

		//         // Check if adjacent voxels have the same color
		//         //if (regionPos[data.pos].Color != regionPos[new Vector3(data.pos.x, data.pos.y, data.pos.z - 1)].Color) continue;

		//         // Check if the sphere is on the same side as the merging direction
		//         bool sphereOnPositiveSide = sphereCollider.transform.position.z > posData[data.pos].z;

		//         if (sphereOnPositiveSide)
		//         {
		//             print("merge z axis  pos");
		//             // Additional check: Only merge if the sphere is on the positive side
		//             if (regionPos[data.pos].z != regionPos[new Vector3(data.pos.x, data.pos.y, data.pos.z - 1)].z) continue;

		//            // regionPos[data.pos] += new Vector3(0, 0, (data.pos.z - 1) + 1);
		//            // regionPos.Remove(new Vector3(data.pos.x, data.pos.y, data.pos.z - 1));



		//}
		//else
		//         {
		//             print("merge z axis  neg");
		//             // Additional check: Only merge if the sphere is on the negative side
		//             //if (regionPos[data.pos].z != regionPos[new Vector3(data.pos.x, data.pos.y, data.pos.z - 1)].z - 1) continue;
		//             print("merge z axis  neg 2");
		//            // regionPos[data.pos] += new Vector3(0, 0, -(data.pos.z - 1) + 1);
		//            // regionPos.Remove(new Vector3(data.pos.x, data.pos.y, data.pos.z - 1));

		//         }
		//  }



		//     // X merging
		//     foreach (var data in VoxelData)
		//     {
		//         if (!regionPos.TryGetValue(data.pos, out myValue)) continue;
		//         if (!regionPos.TryGetValue(new Vector3(data.pos.x - 1, data.pos.y, data.pos.z), out myValue)) continue;
		//         if (regionPos[new Vector3(data.pos.x, data.pos.y, data.pos.z)].z !=
		//	regionPos[new Vector3(data.pos.x - 1, data.pos.y, data.pos.z)].z) continue;

		//         // regionPos[data.pos] += new Vector3((data.pos.x - 1) + 1, 0, 0);
		//         // regionPos.Remove(new Vector3(data.pos.x - 1, data.pos.y, data.pos.z));

		//     }

		//     // Y merging
		//     foreach (var data in VoxelData)
		//     {
		//if (!regionPos.TryGetValue(data.pos, out myValue)) continue;
		//         if (!regionPos.TryGetValue(new Vector3(data.pos.x, data.pos.y - 1, data.pos.z), out myValue)) continue;
		//if (regionPos[data.pos].z != regionPos[new Vector3(data.pos.x, data.pos.y - 1, data.pos.z)].z) continue;
		//         if (regionPos[data.pos].x != regionPos[new Vector3(data.pos.x, data.pos.y - 1, data.pos.z)].x) continue;

		//        // regionPos[data.pos] += new Vector3(0, (data.pos.y - 1) + 1, 0);
		//        // regionPos.Remove(new Vector3(data.pos.x, data.pos.y - 1, data.pos.z));

		//print("merge y axis ");
		//     }

		//Move block outta the freaking way
		//this.gameObject.transform.position = new Vector3(0, 1, 0);
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



//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//               cube.GetComponent<Renderer>().material.color = Color.red;
//               cube.tag = "Box";
//               cube.transform.localScale = new Vector3(
//                   1,
//                   1,
//                   1
//               );