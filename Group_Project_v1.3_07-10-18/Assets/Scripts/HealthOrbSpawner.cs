using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrbSpawner : MonoBehaviour
{
    public GameObject healthOrb;

	void Start()
    {
        Instantiate(healthOrb, this.transform.position + new Vector3(0,1,0), this.transform.rotation);
    }
}
