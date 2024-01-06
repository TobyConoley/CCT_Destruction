using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Coordinate3
{
	public int x { get; set; }
	public int y { get; set; }
	public int z { get; set; }
}


public class GreedyMeshYAxis : MonoBehaviour
{
	public GameObject mesh;

	private int cubesPerXAxis;
	private int cubesPerYAxis;
	private int cubesPerZAxis;

	Dictionary<Vector3Int, Vector3Int> regionPos2 = new Dictionary<Vector3Int, Vector3Int>();
	List<Coordinate3> DataItems = new List<Coordinate3>();


	void Start()
	{
		// Need to round to nearest whole number?
		cubesPerXAxis = (int)this.transform.localScale.x;
		cubesPerYAxis = (int)this.transform.localScale.y + 1; // because it cant start at 0 it has to check voxel behind it at each step to see if can merge
		cubesPerZAxis = (int)this.transform.localScale.z + 1; // + 1
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
		for (int x = 0; x < cubesPerXAxis; x++)
		{
			for (int y = 0; y < cubesPerYAxis; y++)
			{
				for (int z = 0; z < cubesPerZAxis; z++)
				{
					regionPos2[new Vector3Int(x, y, z)] = new Vector3Int(1, 1, 1);
					print("check this yo " + z);
				}
			}
		}

		// Z meshing
		foreach (Vector3Int key in regionPos2.Keys)
		{
			DataItems.Add(new Coordinate3
			{
				x = key.x,
				y = key.y,
				z = key.z,
			});
		}

		//int prevZ = regionPos2[new Vector3Int(data.x, data.y, data.z - 1)].z;
		//int z = data.z;
		//int calcZ = z += prevZ - 1;

		Vector3Int myValue;
		foreach (var data in DataItems)
		{
			if (!regionPos2.TryGetValue(new Vector3Int(data.x, data.y, data.z), out myValue))
			{
				continue;
			}

			if (!regionPos2.TryGetValue(new Vector3Int(data.x, data.y, data.z - 1), out myValue))
			{
				continue;
			}
			
			regionPos2[new Vector3Int(data.x, data.y, data.z)] += new Vector3Int(0, 0, data.z - 1);
			print("how many times does this print 15 I guess? " + regionPos2[new Vector3Int(data.x, data.y, data.z)].z);
			regionPos2.Remove(new Vector3Int(data.x, data.y, data.z - 1));
		}

		// Y meshing
		foreach (var data in DataItems)
		{
			if (!regionPos2.TryGetValue(new Vector3Int(data.x, data.y, data.z), out myValue))
			{
				continue;
			}


			if (!regionPos2.TryGetValue(new Vector3Int(data.x, data.y - 1, data.z), out myValue))
			{
				continue;
			}

			//data.y += regionPos2[new Vector3Int(data.x, data.y - 1, data.z)].y;
			if (data.z != regionPos2[new Vector3Int(data.x, data.y - 1, data.z)].z) continue;
			//if (data.x != regionPos2[new Vector3Int(data.x, data.y - 1, data.z)].x) continue;
		
			regionPos2[new Vector3Int(data.x, data.y, data.z)] += new Vector3Int(0, data.y - 1, 0);
			regionPos2.Remove(new Vector3Int(data.x, data.y - 1, data.z));
		}


		// Create meshes
		//foreach (var data in DataItems)
		foreach (Vector3Int key in regionPos2.Keys)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			print("cube size x " + regionPos2[new Vector3Int(key.x, key.y, key.z)].x + " /y " + regionPos2[new Vector3Int(key.x, key.y, key.z)].y + " /z " + regionPos2[new Vector3Int(key.x, key.y, key.z)].z);
			cube.transform.localScale = new Vector3(
				regionPos2[new Vector3Int(key.x, key.y, key.z)].x, //size.Value["X"]
				regionPos2[new Vector3Int(key.x, key.y, key.z)].y,
				regionPos2[new Vector3Int(key.x, key.y, key.z)].z  //added this to get right size should be easy fix though
			); 

			var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

			var calc = new Vector3(
					key.x - regionPos2[new Vector3Int(key.x, key.y, key.z)].x + cube.transform.localScale.x,
					(key.y - regionPos2[new Vector3Int(key.x, key.y, key.z)].y / 2) - cube.transform.localScale.y / 2, 
					(key.z - regionPos2[new Vector3Int(key.x, key.y, key.z)].z / 2) - cube.transform.localScale.z / 2
				//key.z - regionPos2[new Vector3Int(key.x, key.y, key.z)].z / 2) - cube.transform.localScale.z / 2
				);

			var pos = test + calc;

			cube.transform.position = pos;
		}
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
