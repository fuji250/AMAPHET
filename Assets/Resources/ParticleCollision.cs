using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
	{
		other.gameObject.GetComponent<Renderer>().material.color = Random.ColorHSV();
		Destroy(this, 0f);
		Debug.Log("kieru");
	}
	private void OnTriggerEnter(Collider other)
    {
		Destroy(this, 0f);
		Debug.Log("kieru");
    }
}
