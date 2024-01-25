using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private float VOXEL_SIZE = 0.5f;

    //Input
    public StarterAssets.StarterAssetsInputs starterAssetsInputs;

    // Assuming there's a DestructionModuleV2 class with a SplitPart method
    Test1 destructionModule = new Test1();

    public float damage = 10;
    public float blastRadius;
    public float explosionPower;
    public LayerMask explosionLayers;

    public GameObject Particles;

    private List<GameObject> brokenCubesList = new List<GameObject>();
    private float secondsBeforeCleaned = 5f;
    private bool touched = false;

    void Test()
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        part.transform.position = this.transform.position;
        part.transform.localScale = new Vector3(8, 8, 8);
        part.GetComponent<Renderer>().material.color = Color.green;

        float startTick = Time.time;

        Collider[] colliders = Physics.OverlapBox(part.transform.position, part.transform.localScale / 2);
        foreach (Collider collider in colliders)
        {
            destructionModule.SplitPart(collider.transform, part.transform, VOXEL_SIZE);
        }

        Destroy(part);

        Debug.Log("Total time: " + (Time.time - startTick));
    }

    private void OnCollisionEnter(UnityEngine.Collision col)
    {
        //SpawnParticles(col);

        //if (col.gameObject.CompareTag("Box")) Destroy(col.contacts[0].point);
        if (col.gameObject.layer == LayerMask.NameToLayer("Cube") && !touched)
        {
            touched = true;
            Test();
        }
    }

    //private IEnumerator CleanUp(GameObject cube)
    //{
    //    // Doesn't yield past 5 for some reason?
    //    yield return new WaitForSeconds(secondsBeforeCleaned);
    //    Destroy(cube);
    //    print("Cube destroyed? " + secondsBeforeCleaned);
    //}

    private void SpawnParticles(UnityEngine.Collision col)
    {
        var particles = Particles;
        Particles.gameObject.GetComponent<Renderer>().material = col.gameObject.GetComponent<MeshRenderer>().material;
        Instantiate(Particles, col.transform.position, Quaternion.identity);
    }
}
