using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Testing destruction using dictionarys again

struct Coordinate7
{
	public int x { get; set; }
	public int y { get; set; }
	public int z { get; set; }
}


public class Test : MonoBehaviour
{
	public GameObject mesh;
	public StarterAssets.StarterAssetsInputs starterAssetsInputs;

	//Sphere stuff
	public GameObject sphere;
	SphereCollider sphereCollider;

	public int cubesPerXAxis = 8;
	public int cubesPerYAxis = 8;
	public int cubesPerZAxis = 8;

	public Dictionary<float, Dictionary<float, Dictionary<float, Dictionary<string, float>>>> regionPos =
	new Dictionary<float, Dictionary<float, Dictionary<float, Dictionary<string, float>>>>();

	//Dictionary<Vector3, int> regionPos2 = new Dictionary<Vector3, int>();


	Coordinate7 point = new Coordinate7();
	List<Coordinate7> DataItems = new List<Coordinate7>();

	//Voxel stuff
	int VOXEL_SIZE = 1;

	//Anonymouse function using delegate
	public delegate Vector3 PrintDel(Vector3 vec);

	// divides the size of the part by VOXEL_SIZE
	Vector3 CalculateBlockCount(Vector3 vec)
	{

		var calcVec = new Vector3(
			Mathf.Floor(vec.x / VOXEL_SIZE),
			Mathf.Floor(vec.y / VOXEL_SIZE),
			Mathf.Floor(vec.z / VOXEL_SIZE)
		);
		print("check this man " + calcVec);
		return calcVec;

	}

	// CalculateBlockCount helper function
	Vector3 CalculateBlockCountHelper(Vector3 vec, PrintDel op)
	{
		return op(vec);
	}

	void Start()
	{
		cubesPerXAxis = (int)this.transform.localScale.x;
		cubesPerYAxis = (int)this.transform.localScale.y;
		cubesPerZAxis = (int)this.transform.localScale.z;
		sphere = GameObject.Find("Sphere");
		sphereCollider = sphere.GetComponent<SphereCollider>();
	}

	void test(Vector3 center, float radius)
	{
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		foreach (var hitCollider in hitColliders)
		{
			if (hitCollider.name == "Sphere") continue;
			if (this.gameObject.CompareTag("Box")) CreateCube("Box", this.gameObject);
		}
	}

	//Replaces cube with many smaller cubes
	void CreateCube(string tag, GameObject target)
	{

		Vector3 blockCount = CalculateBlockCountHelper(new Vector3(
		target.transform.localScale.x,
		target.transform.localScale.y,
		target.transform.localScale.z),
		CalculateBlockCount
		);

		Vector3 blockSize = new Vector3(
        //1,
        //1,
        //1
        target.transform.localScale.x / blockCount.x,
        target.transform.localScale.y / blockCount.y,
        target.transform.localScale.z / blockCount.z
        );

		for (int x = 0; x < cubesPerXAxis; x++)
		{
			regionPos[x] = new Dictionary<float, Dictionary<float, Dictionary<string, float>>>();
			for (int y = 0; y < cubesPerYAxis; y++)
			{
				regionPos[x][y] = new Dictionary<float, Dictionary<string, float>>();
				for (int z = 0; z < cubesPerZAxis; z++)
				{

                    Vector3 offset = (target.transform.localScale + blockSize) / 2;
                    Vector3 vecTest = target.transform.TransformDirection(Vector3.Scale(new Vector3(x, y, z), blockSize) - offset);
					Vector3 anotherOne = (target.transform.position + vecTest) + new Vector3(1, 1, 1); //- vecTest // probably + blocksize / 2
					print("offset " + offset );
					print("vecTest pos " + vecTest);
					print("final pos " + anotherOne);

					//GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
     //               cube.GetComponent<Renderer>().material.color = Color.red;
     //               cube.tag = "Box";
     //               cube.transform.localScale = new Vector3(
     //                   1,
     //                   1,
     //                   1
     //               );
					// only pos that works
     //               var test = (this.transform.position - this.transform.localScale / 2) + cube.transform.localScale / 2;

     //               var calc = new Vector3(
     //                    x - 1 + cube.transform.localScale.x,
     //                    (y - 1 / 2) - cube.transform.localScale.y / 2 + 0.5f,
     //                    (z - 1 / 2) - cube.transform.localScale.z / 2 + 0.5f
     //                );

					//Vector3 vecCalc = test + calc;

					float distance = Vector3.Distance(sphereCollider.transform.position, anotherOne);
					if (distance > (sphereCollider.transform.localScale.y / 2))
					{
                       // cube.transform.position = anotherOne;
						regionPos[x][y][z] = new Dictionary<string, float>();
                        regionPos[x][y][z].Add("X", 1);
                        regionPos[x][y][z].Add("Y", 1);
                        regionPos[x][y][z].Add("Z", 1);
					
						regionPos[x][y][z].Add("PX", anotherOne.x);
						regionPos[x][y][z].Add("PY", anotherOne.y);
						regionPos[x][y][z].Add("PZ", anotherOne.z);

						//DataItems.Add(new Coordinate7 { x = x, y = y, z = z });
                        print("part is within the sphere zero " + distance + " / " + sphereCollider.transform.localScale.y / 2);
					}
					else
					{
						print("part is within the sphere " + distance);

					}
				}
			}
		}
		float myValue;
		float testys;
		string cock;
		Dictionary<string, float> yo;
		Dictionary<float, Dictionary<string, float>> yotwo;
		// Create the cubes
		int xx = -1;
		int yy = -1;
		int zz = -1;
		foreach (var row in regionPos)
		{
			xx++;
			if (row.Value == null) continue; //Don't need these?
			foreach (var column in row.Value)
			{
				yy++;
				if (yy < 2) continue;
				if (column.Value == null) continue; //Don't need these?
				foreach (var size in column.Value)
				{
					zz++;
					if (size.Value == null) continue;

                    {
						print("check this bobes " + xx + yy + zz);
						DataItems.Add(new Coordinate7 { x = xx, y = yy, z = zz });
					}
				}
			}
		}

		foreach (var data in DataItems)
		{

            if (!regionPos[data.x].TryGetValue(data.y, out yotwo)) continue;
            if (!regionPos[data.x][data.y].TryGetValue(data.z, out yo)) continue;
            if (!regionPos[data.x][data.y].TryGetValue(data.z - 1, out yo)) continue;
            if (!regionPos[data.x][data.y][data.z].TryGetValue("Z", out testys)) continue;
			print("ever happens? ");
			regionPos[data.x][data.y][data.z]["Z"] += regionPos[data.x][data.y][data.z - 1]["Z"];
			regionPos[data.x][data.y].Remove(regionPos[data.x][data.y][data.z - 1]["Z"]);
		}

		// Create the cubes
		foreach (var row in regionPos)
		{
			if (row.Value == null) continue; //Don't need these?
			foreach (var column in row.Value)
			{
				if (column.Value == null) continue; //Don't need these?
				foreach (var size in column.Value)
				{
					if (size.Value == null) continue;

					print("sizes " + size.Value["X"] + " /y " + size.Value["Y"] + " /z " + size.Value["Z"]);
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

					var calc = Vector3.Scale(new Vector3(size.Value["X"], size.Value["Y"], size.Value["Z"]), blockSize);
					cube.transform.localScale = Vector3.Scale(calc, blockSize);

					cube.transform.position = Vector3.Scale(new Vector3(
						((size.Value["PX"] - size.Value["X"] / 2) + blockSize.x / 2),
						((size.Value["PY"] - size.Value["Y"] / 2) + blockSize.y / 2),
						((size.Value["PZ"] - size.Value["Z"] / 2) + blockSize.z / 2) ), blockSize);

					print("Positions " + cube.transform.position);
				}
			}
		}
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
