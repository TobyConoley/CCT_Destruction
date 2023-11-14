using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
	private Collider[] hitColliders;

	public float blastRadius;
	public float explosionPower;
	public LayerMask explosionLayers;

	public GameObject Particles;

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
		//Spawns particles
		//Particles.gameObject.GetComponent<Renderer>().material = collision.gameObject.GetComponent<MeshRenderer>().material;
		//Instantiate(Particles, collision.transform.position, Quaternion.identity);

		//destroys welds?
		

		destroy(collision.contacts[0].point);
    }

    void destroy(Vector3 explosionPoint)
	{
		hitColliders = Physics.OverlapSphere(explosionPoint, blastRadius, explosionLayers);

		foreach (Collider hitCol in hitColliders)
		{
			if (hitCol.GetComponent<Rigidbody>() == null)
			{
				hitCol.GetComponent<MeshRenderer>().enabled = true;
				hitCol.gameObject.AddComponent<Rigidbody>();


				hitCol.GetComponent<Rigidbody>().mass = 500;
				hitCol.GetComponent<Rigidbody>().isKinematic = false;

				hitCol.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * 5;
				hitCol.GetComponent<Rigidbody>().AddExplosionForce(explosionPower, explosionPoint, blastRadius, 1, ForceMode.Impulse);

				//hitCol.tag = null;
				//hitCol.tag = "Untagged";
				// ^ prevents cubes from flying
			}
		}
	}
}
