using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Coordinate4
{
	public int x { get; set; }
	public int y { get; set; }
	public int z { get; set; }
}


public class GreedyMeshXAxis : MonoBehaviour
{
	public GameObject mesh;

	private int cubesPerXAxis;
	private int cubesPerYAxis;
	private int cubesPerZAxis;

	Dictionary<Vector3Int, Vector3Int> regionPos2 = new Dictionary<Vector3Int, Vector3Int>();
	List<Coordinate4> DataItems = new List<Coordinate4>();

	//Sphere stuff
	public GameObject sphere;
	SphereCollider sphereCollider;

	//Voxel stuff
	int VOXEL_SIZE = 1;

	public delegate Vector3Int PrintDel(Vector3Int vec); // Anonymouse function using delegate
	public StarterAssets.StarterAssetsInputs starterAssetsInputs;

	void Start()
	{
		// Need to round to nearest whole number?
		cubesPerXAxis = (int)this.transform.localScale.x;
		cubesPerYAxis = (int)this.transform.localScale.y + 1; // because it cant start at 0 it has to check voxel behind it at each step to see if can merge
		cubesPerZAxis = (int)this.transform.localScale.z + 1; // + 1
		sphere = GameObject.Find("Sphere");
		sphereCollider = sphere.GetComponent<SphereCollider>();
	}

	private void OnCollisionEnter(UnityEngine.Collision collision)
	{
		if (collision.gameObject.tag == "Projectile")
		{
			//test(sphere.transform.position, sphereCollider.transform.localScale.y);
		}
	}

	// run algorithm anytime I want on the parts colliding with sphere
	void test(Vector3 center, float radius)
    {
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		foreach (var hitCollider in hitColliders)
		{
			print("collided with " + hitCollider);
			if (hitCollider.name == "Sphere") continue;
			if (this.gameObject.CompareTag("Box")) CreateCube("Box", this.gameObject);
		
			//hitCollider.SendMessage("AddDamage");
		}
	}


	// divides the size of the part by VOXEL_SIZE
	Vector3Int CalculateBlockCount(Vector3Int vec) 
    {
		return new Vector3Int(
			(int)Mathf.Floor(vec.x / VOXEL_SIZE),
			(int)Mathf.Floor(vec.x / VOXEL_SIZE),
			(int)Mathf.Floor(vec.x / VOXEL_SIZE)
		);
	}

	Vector3Int CalculateBlockCountHelper(Vector3Int vec, PrintDel op)
    {
		return op(vec);
	}

	void GetClosestPointOnBox()
    {

    }

	//Assign cube data ////////////////////////////////////////////////////////////////////////////////////
	void CreateCube(string tag, GameObject targetPart)
	{
		Vector3Int blockCount = CalculateBlockCountHelper(new Vector3Int(
		(int)targetPart.transform.localScale.x,
		(int)targetPart.transform.localScale.y, 
		(int)targetPart.transform.localScale.y), 
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
						(int)targetPart.transform.localScale.x + blockSize.x / 2,
						(int)targetPart.transform.localScale.y + blockSize.y / 2,
						(int)targetPart.transform.localScale.z + blockSize.z / 2
					);

					targetPart.transform.localToWorldMatrix(new Vector3.new(x, y, z));
					//local cframe = targetPart.CFrame:ToWorldSpace(CFrame.new(Vector3.new(x,y,z) * blockSize - offset))

					//function GetClosestPointOnBox(collider, boxCFrame, boxSize)
					//	local transform = boxCFrame:PointToObjectSpace(collider.Position)
					//	local halfSize = boxSize / 2
					//	local position = boxCFrame * Vector3.new(
					//		math.clamp(transform.X, -halfSize.X, halfSize.X),
					//		math.clamp(transform.Y, -halfSize.Y, halfSize.Y),
					//		math.clamp(transform.Y, -halfSize.Y, halfSize.Y)-- Used to be Y Y Y
					//	)
					//	return position
					//end


					regionPos2[new Vector3Int(x, y, z)] = new Vector3Int(1, 1, 1);
					print("check this yo " + z);
				}
			}
		}

		// Z meshing
		foreach (Vector3Int key in regionPos2.Keys)
		{
			DataItems.Add(new Coordinate4
			{
				x = key.x,
				y = key.y,
				z = key.z,
			});
		}

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

			cube.tag = "Box";
			cube.transform.position = pos;
		}

		//Move block outta the freaking way
		this.gameObject.transform.position = new Vector3(0, 0, 0);
	}



    public void Update()
    {
		if (starterAssetsInputs.explode)
        {
			test(sphere.transform.position, sphereCollider.transform.localScale.y / 2);
			starterAssetsInputs.explode = false;
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

