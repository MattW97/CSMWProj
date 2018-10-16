using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

    public enum ZoneType
    {
        ragdoll,
        killZone
    }

    public ZoneType zoneSelection;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if(zoneSelection == ZoneType.ragdoll)
            {
                if (!other.gameObject.GetComponentInParent<PlayerController>().ragdolling)
                {
                    other.gameObject.GetComponentInParent<PlayerController>().Ragdoll(true);
                }
            }
            else if (zoneSelection == ZoneType.killZone)
            {
                other.gameObject.GetComponentInParent<PlayerController>().isDead = true;
                other.gameObject.SetActive(false);
            }

        }
    }


}
