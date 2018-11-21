using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganExplosionScript : MonoBehaviour
{
    private Rigidbody rigidBody;

    public float force;

    void Start()
    {
        force = 500;
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.AddExplosionForce(force, transform.position + Random.insideUnitSphere * 5, 20, 20);
    }
}