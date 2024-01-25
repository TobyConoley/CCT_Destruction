using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientTest : MonoBehaviour
{
    public Camera camera;
    public LayerMask rayMask;

    public float VOXEL_SIZE = 0.5f;

    //Input
     public StarterAssets.StarterAssetsInputs starterAssetsInputs;

     Test1 destructionModule = new Test1();

    void Update()
    {
        if (starterAssetsInputs.explode)
        {
            starterAssetsInputs.explode = false;
            Click();
        }
    }

    void Click()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 1000f, rayMask)) 
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            part.transform.position = hit.point;
            part.transform.localScale = new Vector3(5, 5, 5) * 2;
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
    }

}
