using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct Coordinate2
{
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
	public Vector3Int vec { get; set; }
	public Vector3Int vec2 { get; set; }
}


public class GreedyMesh : MonoBehaviour
{
	public GameObject mesh;

	public int cubesPerXAxis = 2;
	public int cubesPerYAxis = 2;
	public int cubesPerZAxis = 3;

	//public Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, int>>>> regionPos =
	//new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, int>>>>();

	Dictionary<Vector3Int, Vector3Int> regionPos2 = new Dictionary<Vector3Int, Vector3Int>();
	List<Coordinate2> DataItems = new List<Coordinate2>();


	void Start()
	{
		// Need to round to nearest whole number?
		cubesPerXAxis = (int)this.transform.localScale.x;
		cubesPerYAxis = (int)this.transform.localScale.y;
		cubesPerZAxis = (int)this.transform.localScale.z;
	}

	private void OnCollisionEnter(UnityEngine.Collision collision)
	{
		if (collision.gameObject.tag == "Projectile")
		{
			if (this.gameObject.CompareTag("Box")) CreateCube("Box");
		}
	}

	//Replaces cube with many smaller cubes
	void CreateCube(string tag)
	{
		// Rotated objects seem to spawn under the map
		// probably caused by box collisions causing weird physics
		for (int x = 0; x < cubesPerXAxis; x++)
		{
			//regionPos[x] = new Dictionary<int, Dictionary<int, Dictionary<string, int>>>();
			for (int y = 0; y < cubesPerYAxis; y++)
			{
				//regionPos[x][y] = new Dictionary<int, Dictionary<string, int>>();
				for (int z = 0; z < cubesPerZAxis; z++)
				{
					// The problem with the size of cubes is that changeing the
					// z size changes all the cubes sizes since it only has one value for key
					regionPos2[new Vector3Int(x, y, z)] = new Vector3Int(1, 1, 1);


					//regionPos[x][y][z] = new Dictionary<string, int>();
					//regionPos[x][y][z].Add("X", 1);
					//regionPos[x][y][z].Add("Y", 1);
					//regionPos[x][y][z].Add("Z", 1);
				}
			}
		}

		
		Vector3Int myValue;
		//int iterator = -1;S
		foreach (Vector3Int key in regionPos2.Keys)
		{
			print("values in dictionary " + regionPos2[key] + " / " + regionPos2[key].x);
			if (!regionPos2.TryGetValue(new Vector3Int(key.x, key.y, key.z - 1), out myValue)) 
			{
				print("Continue brother " + key);
				continue;
			}
			else
            {
				print("check this man " + key);
				DataItems.Add(new Coordinate2
				{
					x = key.x,
					y = key.y,
					z = key.z,
				});
			}
		}

		foreach (var data in DataItems)
		{
			//int yo = data.z;
			//int var = (int)Mathf.Clamp(yo - 1, 1, Mathf.Infinity);
			int z = data.z;
			int calcZ = z += z - 1;
	
			//print("is this any dif " + calcZ);
			regionPos2[new Vector3Int(data.x, data.y, data.z)] += new Vector3Int(0, 0, calcZ); //Only modifys the y value instead of all xyz values in dict

			//print("not present in dict? One" + data.x + data.y + data.z);//regionPos2[new Vector3Int(data.x, data.y, data.z)] += regionPos2[new Vector3Int(data.x, data.y, calcZ)]; 
			//print("not present in dict? Two" + regionPos2[new Vector3Int(data.x, data.y, calcZ)]);
			//It's not present in the dictionary because it hasn't been adde dto dicitonary yet?
			//Trying to remove 3 as calcZ but 3 doesn't exist in the dict yet?
			bool check = regionPos2.Remove(regionPos2[new Vector3Int(data.x, data.y, calcZ)]); //regionPos2[new Vector3Int(data.x, data.y, calcZ)]
			//print("remove check " + check);
			//DataItems.Remove(data);
		}

		//Iterates 8 times need to get it down to 4 
		//foreach (Vector3Int row in regionPos2.Keys)
		//int iterator = -1;
		foreach (var row in DataItems) //data
			{
			//if (!regionPos2.TryGetValue(new Vector3Int(row.x, row.y, row.z), out myValue)) continue;

			//iterator++;
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			print("cube size x " + regionPos2[new Vector3Int(row.x, row.y, row.z)].x + " /y " + regionPos2[new Vector3Int(row.x, row.y, row.z)].y  +  " /z " + regionPos2[new Vector3Int(row.x, row.y, row.z)].z);
			cube.transform.localScale = new Vector3(
				regionPos2[new Vector3Int(row.x, row.y, row.z)].x, //size.Value["X"]
				regionPos2[new Vector3Int(row.x, row.y, row.z)].y,
				regionPos2[new Vector3Int(row.x, row.y, row.z)].z
			);

			var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

			var calc = new Vector3(
					row.x  - regionPos2[new Vector3Int(row.x, row.y, row.z)].x + cube.transform.localScale.x,
					row.y - regionPos2[new Vector3Int(row.x, row.y, row.z)].y + cube.transform.localScale.y,
					row.z - regionPos2[new Vector3Int(row.x, row.y, row.z)].z + cube.transform.localScale.z / 2
				//row.Key - size.Value["X"] + cube.transform.localScale.x,
				//column.Key - size.Value["Y"] + cube.transform.localScale.y,
				//size.Key - size.Value["Z"] + cube.transform.localScale.z / 2
				);

			var pos = test + calc;

			cube.transform.position = pos;

			Debug.Log("Does this work toby ");
		}

		
		//foreach (Vector3Int row in regionPos2.Keys)
		//{
		//	GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//	print("cube size x " + regionPos2[row].x + " /y " + regionPos2[row].y + " /z " + regionPos2[row].z);
		//	cube.transform.localScale = new Vector3(
		//		regionPos2[row].x, //size.Value["X"]
		//		regionPos2[row].y,
		//		regionPos2[row].z
		//	);

		//	var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

		//	var calc = new Vector3(
		//			row.x + cube.transform.localScale.x,
		//			row.y + cube.transform.localScale.y,
		//			row.z + cube.transform.localScale.z / 2
		//		//row.Key - size.Value["X"] + cube.transform.localScale.x,
		//		//column.Key - size.Value["Y"] + cube.transform.localScale.y,
		//		//size.Key - size.Value["Z"] + cube.transform.localScale.z / 2
		//		);

		//	var pos = test + calc;

		//	cube.transform.position = pos;

		//	Debug.Log("Does this work toby ");
		//}


		// Merge z voxels
		//int _x = -1;
		//int _y = -1;
		//int _z = -1;
		//int testys = 0;
		//foreach (int row in regionPos.Keys)
		//{
		//	_x++;
		//	foreach (int column in regionPos[row].Keys)
		//	{
		//		_y++;
		//		foreach (int size in regionPos[column].Keys)
		//		{
		//			_z++;
		//			;
		//			if (!regionPos[_x][_y][_z].TryGetValue("Z", out testys)) continue; //regionPos[_x][_y][_z] == null
		//			if (!regionPos[_x][_y].ContainsKey(_z - 1)) continue;

		//			regionPos[_x][_y][_z]["Z"] += regionPos[_x][_y][_z - 1]["Z"];
		//			regionPos[_x][_y][_z - 1].Clear();

		//		}
		//	}
		//}


		//foreach (var row in regionPos)
		//{
		//	if (row.Value == null) continue; //Don't need these?
		//	foreach (var column in row.Value)
		//	{
		//		if (column.Value == null) continue; //Don't need these?
		//		foreach (var size in column.Value)
		//		{
		//			if (size.Value == null) continue;

		//			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

		//			cube.transform.localScale = new Vector3(
		//				size.Value["X"],
		//				size.Value["Y"],
		//				size.Value["Z"]
		//			);

		//			var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

		//			var calc = new Vector3(
		//					row.Key - size.Value["X"] + cube.transform.localScale.x,
		//					column.Key - size.Value["Y"] + cube.transform.localScale.y,
		//					size.Key - size.Value["Z"] + cube.transform.localScale.z / 2
		//				);

		//			var pos = test + calc;

		//			cube.transform.position = pos;

		//			Debug.Log("size " + size.Value["X"]);
		//		}
		//	}
		//}
	}


	private void AddRigidBody(GameObject cube)
	{
		Rigidbody rb = cube.AddComponent<Rigidbody>();
		//rb.useGravity = false;
		rb.mass = 500;
		rb.isKinematic = false;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

	}
}
