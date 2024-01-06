using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need to implement a greedy mesh algorithm
//generating box colliders using a greedy algorithm.

//Step 1. By clicking it does a raycast into the world and when hitting a voxel it destroys all voxels within a dynamic radius.

// The hardest thing was to detect when an object should be split into multiple objects. On paper this sounds so easy,
// but making the engine understand this was quite hard to optimize.

struct Coordinate
{
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
}


public class Destruction : MonoBehaviour
{
	public GameObject mesh;

	public int cubesPerXAxis = 8;
	public int cubesPerYAxis = 8;
	public int cubesPerZAxis = 8;

	public Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, int>>>> regionPos =
	new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, int>>>>();

	Dictionary<Vector3, int> regionPos2 = new Dictionary<Vector3, int>();


	Coordinate point = new Coordinate();
	List<Coordinate> DataItems = new List<Coordinate>();


	void Start()
	{
		// Need to round to nearest whole number?
		cubesPerXAxis = (int)this.transform.localScale.x;
		cubesPerYAxis = (int)this.transform.localScale.y;
		cubesPerZAxis = (int)this.transform.localScale.z;

		//cubesPerXAxis = 2; //(int)this.transform.localScale.x;
		//cubesPerYAxis = 2; //(int)this.transform.localScale.y;
		//cubesPerZAxis = 2; //(int)this.transform.localScale.z;
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
		for (int x = 0; x < cubesPerXAxis;  x++) 
		{
			regionPos[x] = new Dictionary<int, Dictionary<int, Dictionary<string, int>>>();
			for (int y = 0; y < cubesPerYAxis; y++) 
			{
				regionPos[x][y] = new Dictionary<int, Dictionary<string, int>>();
				for (int z = 0; z < cubesPerZAxis; z++) 
				{
					regionPos[x][y][z] = new Dictionary<string, int>();
					regionPos[x][y][z].Add("X", 1);
					regionPos[x][y][z].Add("Y", 1);
					regionPos[x][y][z].Add("Z", 1);
					//var testyyy = new Vector3Int(x, y, z);
					regionPos2[new Vector3Int(x, y, z)] = 1;
					//print("new method " + regionPos2[new Vector3Int(x, y, z)]);
					//regionPos[x][y][z].Add("Position", 1);
					//Debug.Log("Check the dictionary works " + regionPos[x][y][z]["X"] + regionPos[x][y][z]["Y"] + regionPos[x][y][z]["Z"]);

					//regionPos[x][y][z] = {["X"] = 1, ["Y"] = 1, ["Z"] = 1, ["Position"] = cframe.Position}
				}
            }
        }

		int myValue = 0;
		//int iterator = -1;S
		foreach(Vector3 value in regionPos2.Keys)
        {
			print("values in dictionary " + regionPos2[value]);
			//if (regionPos2[value] == null) //regionPos2[value] == null
			//{
			//	continue;
   //         }
			if (!regionPos2.TryGetValue(new Vector3(value.x, value.y, value.z - 1), out myValue))
            {
				continue;
            }
			//Vector3 vec = new Vector3(value.x, value.y, value.z);
			//Vector3 vec2 = new Vector3(value.x, value.y, value.z - 1);

			DataItems.Add(new Coordinate {x = value.x, y = value.y, z = value.z});

			//regionPos2[vec] += regionPos2[vec2];
			//regionPos2[new Vector3(value.x, value.y, value.z - 1)] = 0;
			//regionPos2.Remove(new Vector3(value.x, value.y, value.z - 1));
		}

		foreach (var data in DataItems)
		{
			Vector3 vec = new Vector3(data.x, data.y, data.z);
			Vector3 vec2 = new Vector3(data.x, data.y, data.z - 1);
			regionPos2[vec] += regionPos2[vec2];
			//regionPos2[new Vector3(data.x, data.y, data.z - 1)] = 0;
			regionPos2.Remove(new Vector3(data.x, data.y, data.z - 1));
			print("data ");
		}

		//Iterates 16 times need to get it down to 4 
		foreach (Vector3 row in regionPos2.Keys)
        {
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			print("cube size " + regionPos2[row]);
			cube.transform.localScale = new Vector3(
				regionPos2[row], //size.Value["X"]
				regionPos2[row],
				regionPos2[row]
			);

			var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

			var calc = new Vector3(
					row.x + cube.transform.localScale.x,
					row.y + cube.transform.localScale.y,
					row.z + cube.transform.localScale.z / 2
				//row.Key - size.Value["X"] + cube.transform.localScale.x,
				//column.Key - size.Value["Y"] + cube.transform.localScale.y,
				//size.Key - size.Value["Z"] + cube.transform.localScale.z / 2
				);

			var pos = test + calc;

			cube.transform.position = pos;

			Debug.Log("Does this work toby ");
		}
            

			// Merge z voxels
			int _x = -1;
		int _y = -1;
		int _z = -1;
		int testys = 0;
		foreach (int row in regionPos.Keys)
        {
			_x++;
			foreach (int column in regionPos[row].Keys)
            {
				_y++;
				foreach (int size in regionPos[column].Keys)
				{
					_z++;
; 
					if (!regionPos[_x][_y][_z].TryGetValue("Z", out testys)) continue; //regionPos[_x][_y][_z] == null
					if (!regionPos[_x][_y].ContainsKey(_z - 1)) continue;

					regionPos[_x][_y][_z]["Z"] += regionPos[_x][_y][_z - 1]["Z"];
					regionPos[_x][_y][_z - 1].Clear();
			
				}
			}
		}

		int zz = 0;
		//Merge X voxels
		//for (int x = 0; x < cubesPerXAxis; x++)
  //      {
  //          for (int y = 0; y < cubesPerYAxis; y++)
  //          {
		//			foreach (var size in regionPos[x]) 
  //                  {
		//				print("Z value " + zz);
		//				//if (size.Value == null) continue; // may or may not need this
  //                      if (size.Value[x] == null)
  //                      {
  //                          Debug.Log("Continue zero ");
  //                          continue;
  //                      }


  //                      if (regionPos[x][y][zz] == null) // include
  //                      {
  //                          Debug.Log("Continue one ");
  //                          continue;
  //                      }

		//				if (regionPos[x - 1][y][zz] == null) {
		//					Debug.Log("Continue two ");
		//					continue;
		//				}; // include


  //                  //	if (!regionPos.ContainsKey(x - 1)) //regionPos.ContainsKey(x - 1)
  //                  //{
  //                  //			Debug.Log("Continue four ");
  //                  //			continue;
  //                  //		}


  //                  if (size.Value[x]["Z"] != regionPos[x - 1][y][zz]["Z"])
  //                      {
  //                          Debug.Log("Continue three ");
  //                          continue;
  //                      }
		//				Debug.Log("reached destiantion ");
  //                      size.Value[x]["X"] += regionPos[x - 1][y][zz]["X"];
  //                      regionPos[x - 1][y][zz] = null;



		//			//if not regionPos[x][y][z] then continue end

		//			//if regionPos[x - 1][y][z] == nil then continue end

		//			//if size.Z ~= regionPos[x - 1][y][z].Z then continue end


		//			//size.X += regionPos[x - 1][y][z].X

		//			//regionPos[x - 1][y][z] = nil
		//			zz++;

		//		}
  //          }
  //      }

		// Merge Y voxels
		//zz = -1;
		//int xx = -1;
		//foreach (var row in regionPos)
		//{
		//	xx++;
		//	//if (row.Value == null) continue; //Don't need these?
		//	for (int y = 1; y < cubesPerYAxis; y++)
		//	{
		//		foreach (var size in regionPos[xx][y])
  //              {
		//			zz++;
		//			//if (size.Value == null) continue; //Don't need these?
		//			print("Test zero " + size.Value); //+ xx + " / " + y + " / " + zz
		//			if (!regionPos[xx][y].ContainsKey(zz)) continue; //regionPos[xx][y][zz] == null //!regionPos[xx][y].ContainsKey(zz

		//			print("Test one");
  //                  if (!regionPos[xx].ContainsKey(y - 1)) continue; //regionPos[xx][y - 1][zz] == null

		//			print("Test two ");

  //                  if (size.Value["Z"] != regionPos[xx][y - 1][zz]["Z"]) continue;

		//			print("Test three");
		//			if (size.Value["X"] != regionPos[xx][y - 1][zz]["X"]) continue;


		//			print("Test four " + size.Value["Y"]);
		//			print("Test fourr " + regionPos[xx][y - 1][zz]["Y"]);
		//			size.Value["Y"] += regionPos[xx][y - 1][zz]["Y"];
		//			regionPos[xx][y - 1][zz] = null;
		//		}
		//	}
		//}



			//	for x, row in pairs(sizes) do
			//		for y = 2, cubesPerYAxis do
			//			for z, size in pairs(sizes[x][y]) do

			//			if not sizes[x][y][z] then continue end // done
			//			if sizes[x][y - 1][z] == nil then continue end // done
			//			if size.Z ~= sizes[x][y - 1][z].Z then continue end // done
			//			if size.X ~= sizes[x][y - 1][z].X then continue end // done
			//			size.Y += sizes[x][y - 1][z].Y // done
			//			sizes[x][y - 1][z] = nil // done
			//		end
			//	end
			//end



			foreach (var row in regionPos)
		{
			if (row.Value == null) continue; //Don't need these?
			foreach (var column in row.Value)
			{
				if (column.Value == null) continue; //Don't need these?
				foreach (var size in column.Value)
                {
					if (size.Value == null) continue;
					
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

					cube.transform.localScale = new Vector3(
						size.Value["X"], 
						size.Value["Y"],
						size.Value["Z"] 
					); 

					var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

					var calc = new Vector3(
							row.Key - size.Value["X"] + cube.transform.localScale.x, 
							column.Key - size.Value["Y"] + cube.transform.localScale.y,
							size.Key - size.Value["Z"] + cube.transform.localScale.z / 2
						);

					var pos = test + calc;

					cube.transform.position = pos;

					Debug.Log("size " + size.Value["X"]);
				}
			}
		}
		//local test = part.Position - part.Size / 2 + cube.Size / 2
		//local calc = Vector3.new(x - size.X, y - size.Y, z - size.Z)-- * cube.Size
		//local position = test + calc


		//cube.CFrame = CFrame.new(position)--x - size.X / 2, position.Y, z - size.Z / 2
		//cube.Parent = workspace

		// create cube voxels
		//foreach (int row in regionPos.Keys)
		//{
		//	foreach (int column in regionPos[row].Keys)
		//	{
		//		foreach (int size in regionPos[column].Keys)
		//		{
		//			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

		//			cube.transform.localScale = new Vector3(
		//				size, //size //regionPos[row][column][size]["X"]
		//				size,//size
		//				size //size
		//			); // transform.localScale / cubesPerAxis;

		//			//Debug.Log("local size " + regionPos[row][column][size].ContainsKey("X"));

		//			var test = this.transform.position - this.transform.localScale / 2 + cube.transform.localScale;
		//			var test2 = new Vector3(row - size, column - size, size - size);
		//			var pos = test + test2;

		//			cube.transform.position = pos;

		//		}
		//	}
		//}

		//Destroy(this.gameObject);
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
