using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private Collider[] hitColliders;

    public float damage = 10;
    public float blastRadius;
    public float explosionPower;
    public LayerMask explosionLayers;

    public GameObject Particles;

    private List<GameObject> brokenCubesList = new List<GameObject>();
    private float secondsBeforeCleaned = 5f;

    private void OnCollisionEnter(UnityEngine.Collision col)
    {
        //SpawnParticles(col);

        //if (col.gameObject.CompareTag("Box")) Destroy(col.contacts[0].point);
        if (col.gameObject.CompareTag("Box"))
        {
            GameObject cube = col.gameObject;
            Rigidbody rb = cube.GetComponent<Rigidbody>();

            //Skips objects E.g. cubes that don't have a rb because they are in process of being destroyed?
            //Maybe tag object is a better way to handle this in future?
            if (!rb)
                return;

            //rb.AddExplosionForce(explosionPower, col.contacts[0].point, blastRadius, 1, ForceMode.Impulse);
            StartCoroutine(CleanUp(cube));
        }
    }

    // This function adds force to every cube where as the new funciton only adds the force to cubes that where hit so the cubes 
    // Don't explode all at once
    //add explosion force to cubes within contact point
    //void Destroy(Vector3 explosionPoint)
    //{
    //    hitColliders = Physics.OverlapSphere(explosionPoint, blastRadius); //, explosionLayers

    //    foreach (Collider hitCol in hitColliders)
    //    {
    //        if (hitCol.GetComponent<Rigidbody>())
    //        {
    //            GameObject cube = hitCol.gameObject;
    //            Rigidbody rb = cube.GetComponent<Rigidbody>();
    //            rb.AddExplosionForce(explosionPower, explosionPoint, blastRadius, 1, ForceMode.Impulse);
    //            print("Explode " + cube + " // " + rb);
    //           // StartCoroutine(SelfDestruct(cube));
    //        }
    //    }
    //}

    // Break cubes apart
    //IEnumerator SelfDestruct(GameObject cube)
    //{
    //    print("Cube destroyed? 0");
    //    yield return new WaitForSeconds(5f);
    //    print("Cube destroyed? 1");
    //    Destroy(cube);
    //    print("Cube destroyed? 2");
    //}

    private IEnumerator CleanUp(GameObject cube)
    {
        // Doesn't yield past 5 for some reason?
        yield return new WaitForSeconds(secondsBeforeCleaned);
        Destroy(cube);
        print("Cube destroyed? " + secondsBeforeCleaned);
    }

    private void SpawnParticles(UnityEngine.Collision col)
    {
        var particles = Particles;
        Particles.gameObject.GetComponent<Renderer>().material = col.gameObject.GetComponent<MeshRenderer>().material;
        Instantiate(Particles, col.transform.position, Quaternion.identity);
    }
}
