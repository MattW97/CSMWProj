using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    private bool isOccupied;

    private int weaponIndex;

    private float respawnTimer;
    private float initRespawnTime = 15;

    public List<GameObject> weapons;

    private Transform thisTransform;

    public GameObject lightObject;

    void Start()
    {
        respawnTimer = initRespawnTime;

        thisTransform = GetComponent<Transform>();

        weaponIndex = Random.Range(0, weapons.Count);
        Quaternion weaponRotation = weapons[weaponIndex].transform.rotation * Quaternion.Euler(10, 0, 0);
        Instantiate(weapons[weaponIndex], this.transform.position + new Vector3(0, 0.5f, 0), weaponRotation, gameObject.transform);
    }

    void Update()
    {
        if(thisTransform.childCount == 0)
        {
            lightObject.SetActive(false);

            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0)
            {
                weaponIndex = Random.Range(0, weapons.Count);
                Quaternion weaponRotation = weapons[weaponIndex].transform.rotation * Quaternion.Euler(10, 0, 0);
                Instantiate(weapons[weaponIndex], this.transform.position + new Vector3(0, 0.5f, 0), weaponRotation, gameObject.transform);

                lightObject.SetActive(true);

                respawnTimer = initRespawnTime;
            }
        }
    }
}

