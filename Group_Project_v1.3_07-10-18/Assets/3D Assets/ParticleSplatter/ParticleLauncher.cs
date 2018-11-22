using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLauncher : MonoBehaviour {

    private ParticleSystem particleLauncher;
    public GameObject splatDecalPool;

    private float decalSizeMin = 0.5f;
    private float decalSizeMax = 1.0f;

    List<ParticleCollisionEvent> collisionEvents;

	void Start ()
    {
        particleLauncher = GetComponent<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();
	}

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleLauncher, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            Vector3 pos = collisionEvents[i].intersection;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, collisionEvents[i].normal);

            GameObject splat;
            splat = Instantiate(splatDecalPool, pos, rot);

            float randSize = Random.Range(decalSizeMin, decalSizeMax);

            splat.transform.localScale = new Vector3(randSize, 1, randSize);

            if(other.tag == "Trap")
            {
                splat.transform.parent = other.transform;
            }
        }
    }
}
