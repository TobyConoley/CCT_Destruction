using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoxelData
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    public float xSize { get; set; }
    public float ySize { get; set; }
    public float zSize { get; set; }
}

public class Test1: MonoBehaviour
{
    //Input
    public StarterAssets.StarterAssetsInputs starterAssetsInputs;

    //Sphere stuff
    public GameObject sphere;
    SphereCollider sphereCollider;
    void Start()
    {
        sphere = GameObject.Find("Sphere");
        sphereCollider = sphere.GetComponent<SphereCollider>();
    }

    private Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    public void SplitPart(Transform targetPart, Transform collider, float voxelSize)
    {
        Dictionary<float, Dictionary<float, Dictionary<float, VoxelData>>> regionPos = new Dictionary<float, Dictionary<float, Dictionary<float, VoxelData>>>();

        Bounds bounds = new Bounds(targetPart.position, targetPart.localScale);
        Collider[] hitColliders = Physics.OverlapBox(bounds.center, bounds.extents / 2, targetPart.rotation);

        float[] blockCount = new float[3];
        for (int i = 0; i < 3; i++)
        {
            blockCount[i] = Mathf.FloorToInt(bounds.size[i] / voxelSize);
        }

        //Remove this v
        // Convert each element in the blockCount array to int
        for (int i = 0; i < 3; i++)
        {
            blockCount[i] = (float)blockCount[i];
        }

        Vector3 blockSize = new Vector3(voxelSize, voxelSize, voxelSize);
        //Vector3 blockSize = new Vector3(bounds.size.x / blockCount[0], bounds.size.y / blockCount[1], bounds.size.z / blockCount[2]) / voxelSize;
        print("blockSize " + blockSize);
        // Vector3 blockSize = new Vector3(bounds.size.x / blockCount[0], bounds.size.y / blockCount[1], bounds.size.z / blockCount[2]);

        // Initialize a marker for connected voxels
        int marker = 1;

        // Dictionary to store the marker for each voxel position
        Dictionary<Vector3Int, int> voxelMarkers = new Dictionary<Vector3Int, int>();


        for (float x = 1; x <= blockCount[0]; x++)
        {
            regionPos[x] = new Dictionary<float, Dictionary<float, VoxelData>>();
            for (float y = 1; y <= blockCount[1]; y++)
            {
                regionPos[x][y] = new Dictionary<float, VoxelData>();
                for (float z = 1; z <= blockCount[2]; z++)
                {
                    Vector3 offset = (bounds.size + blockSize) / 2;

                    Vector3 translation = Vector3.Scale(new Vector3(x, y, z), blockSize);
                    Matrix4x4 cframe = Matrix4x4.TRS(translation - offset, Quaternion.identity, Vector3.one) * targetPart.localToWorldMatrix;

                    Vector3 position = cframe.MultiplyPoint3x4(Vector3.zero);
           
                    float distance = (collider.position - position).magnitude;

                    // Voxels outside of the collision sphere
                    if (distance > collider.localScale.y / 2)
                    {
                        regionPos[x][y][z] = new VoxelData
                        {
                            x = position.x,
                            y = position.y,
                            z = position.z,

                            xSize = voxelSize,
                            ySize = voxelSize,
                            zSize = voxelSize,
                        };

                        // Mark the voxel as connected
                        voxelMarkers[new Vector3Int((int)x, (int)y, (int)z)] = marker;
                    }
                    else
                    {
                        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        part.GetComponent<Renderer>().material.color = targetPart.GetComponent<Renderer>().material.color;
                        part.name = "Test";
                        part.tag = "Box";
                        part.layer = LayerMask.NameToLayer("Cube");
                        part.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);

                       // part.transform.position = position;
                    }
                }
            }
        }

        // Merge z voxels
        for (float x = 1; x <= blockCount[0]; x++)
        {
            for (float y = 1; y <= blockCount[1]; y++)
            {
                for (float z = 1; z <= blockCount[2]; z++)
                {
                    if (!regionPos[x][y].ContainsKey(z)) continue;
                    if (!regionPos[x][y].ContainsKey(z - 1)) continue;

                    regionPos[x][y][z].zSize += regionPos[x][y][z - 1].zSize;
                    bool remove = regionPos[x][y].Remove(z - 1);
                }
            }
        }

        // Merge x voxels
        for (float x = 2; x <= blockCount[0]; x++)
        {
            for (float y = 1; y <= blockCount[1]; y++)
            {
                foreach (var entry in regionPos[x][y])
                {
                    float z = entry.Key;
                    var size = entry.Value;

                    if (regionPos.ContainsKey(x) && regionPos[x][y].ContainsKey(z) && size != null)
                    {
                        if (x > 2 && regionPos.ContainsKey(x - 1) && regionPos[x - 1].ContainsKey(y) && regionPos[x - 1][y].ContainsKey(z) && size.zSize == regionPos[x - 1][y][z].zSize)
                        {
                            size.xSize += regionPos[x - 1][y][z].xSize;
                            regionPos[x - 1][y].Remove(z);

                            // remove the entire inner dictionary if it's empty
                            if (regionPos[x - 1][y].Count == 0)
                            {
                                regionPos[x - 1].Remove(y);
                            }
                        }
                    }
                }
            }
        }

        // Merge Y voxels
        List<float> keysToRemove = new List<float>();
        foreach (var x in regionPos.Keys)
        {
            for (float y = 2; y <= blockCount[1]; y++)
            {
                if (!regionPos[x].ContainsKey(y)) continue;
                foreach (var entry in regionPos[x][y])
                {
                    float z = entry.Key;
                    var size = entry.Value;

                    if (!regionPos[x].ContainsKey(y) || !regionPos[x][y].ContainsKey(z)) continue;
                    if (!regionPos[x].ContainsKey(y - 1) || !regionPos[x][y - 1].ContainsKey(z)) continue;
                    if (size.zSize != regionPos[x][y - 1][z].zSize) continue;
                    if (size.xSize != regionPos[x][y - 1][z].xSize) continue;

                    size.ySize += regionPos[x][y - 1][z].ySize;
                    keysToRemove.Add(y - 1);
                }
            }

            foreach (float keyToRemove in keysToRemove)
            {
                regionPos[x].Remove(keyToRemove);
            }

            keysToRemove.Clear(); // Clear the list for the next iteration
        }

        foreach (var x in regionPos.Keys)
        {
            foreach (var y in regionPos[x].Keys)
            {
                foreach (var z in regionPos[x][y].Keys)
                {
                    Vector3 position = new Vector3(regionPos[x][y][z].x, regionPos[x][y][z].y, regionPos[x][y][z].z);
                    Vector3 size = new Vector3(regionPos[x][y][z].xSize, regionPos[x][y][z].ySize, regionPos[x][y][z].zSize);

                    GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    part.GetComponent<Renderer>().material.color = targetPart.GetComponent<Renderer>().material.color;
                    part.name = "Test";
                    part.tag = "Box";
                    part.layer = LayerMask.NameToLayer("Cube");

                    part.transform.position = new Vector3(
                        (position.x - size.x / 2) + blockSize.x / 2,
                        (position.y - size.y / 2) + blockSize.y / 2,
                        (position.z - size.z / 2) + blockSize.z / 2
                    );

                    part.transform.localScale = Vector3.Scale(new Vector3(size.x, size.y, size.z), blockSize);
                }
            }
        }

        GameObject.Destroy(targetPart.gameObject);

        //foreach (var x in regionPos.Keys)
        //{
        //    foreach (var y in regionPos[x].Keys)
        //    {
        //        foreach (var z in regionPos[x][y].Keys)
        //        {
        //            Vector3Int currentVoxel = new Vector3Int(x, y, z);

        //            // If the voxel is not marked, perform flood-fill
        //            if (!voxelMarkers.ContainsKey(currentVoxel) || voxelMarkers[currentVoxel] != marker)
        //            {
        //                FloodFill(voxelMarkers, currentVoxel, marker, blockCount);
        //            }
        //        }
        //    }
        //}

        // Now, any voxel not marked is floating
       // DetectFloatingVoxels(voxelMarkers, blockCount);
    }

    // Flood-fill algorithm (as previously provided)
    private void FloodFill(Dictionary<Vector3Int, int> voxelMarkers, Vector3Int currentVoxel, int marker, int[] blockCount)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(currentVoxel);

        while (queue.Count > 0)
        {
            Vector3Int voxel = queue.Dequeue();
            voxelMarkers[voxel] = marker;

            // Define the neighbors for a 3D grid
            Vector3Int[] neighbors = new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
                // Add more neighbors if needed
            };

            foreach (Vector3Int neighborOffset in neighbors)
            {
                Vector3Int neighbor = voxel + neighborOffset;

                if (IsValidVoxel(neighbor, blockCount) && !voxelMarkers.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    private bool IsValidVoxel(Vector3Int voxel, int[] blockCount)
    {
        return voxel.x >= 0 && voxel.y >= 0 && voxel.z >= 0 && voxel.x < blockCount[0] && voxel.y < blockCount[1] && voxel.z < blockCount[2];
    }

    private void DetectFloatingVoxels(Dictionary<Vector3Int, int> voxelMarkers, int[] blockCount)
    {
        for (int x = 1; x <= blockCount[0]; x++)
        {
            for (int y = 1; y <= blockCount[1]; y++)
            {
                for (int z = 1; z <= blockCount[2]; z++)
                {
                    Vector3Int currentVoxel = new Vector3Int(x, y, z);

                    // Check if the voxel is not marked
                    if (!voxelMarkers.ContainsKey(currentVoxel))
                    {
                        // This voxel is floating, you can take appropriate action
                       // Debug.Log("Floating Voxel at " + currentVoxel.ToString());
                    }
                }
            }
        }
    }
}


 
