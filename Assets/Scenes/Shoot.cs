using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    public GameObject projectile;
    public Transform point; // point in player model where projectile will spawn
    public int speed;
    public StarterAssets.StarterAssetsInputs starterAssetsInputs;  
    
    void Update()
    {
        // print("Get input please zero " + starterAssetsInputs.fire);
        /// poll for input 
        if (starterAssetsInputs.fire) //Input.GetMouseButton(0)
        {
            print("Get input please one " + starterAssetsInputs.fire);
            starterAssetsInputs.fire = false;
            GameObject bul = (GameObject)Instantiate(projectile, point.transform.position, Quaternion.identity);
            bul.gameObject.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * speed;
            print("Get input please two");
        }
    }
}
