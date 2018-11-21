using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    private bool isOccupied;
    private int weaponIndex;

    public List<GameObject> weapons;

    void Start()
    {
        weaponIndex = Random.Range(0, weapons.Count);
        Instantiate(weapons[weaponIndex], this.transform.position + new Vector3(0, 1, 0), Quaternion.identity);       
    }
}

