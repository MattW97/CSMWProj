using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class KillZone : MonoBehaviour
{

    public enum ZoneType
    {
        ragdoll,
        killZone,
        both,
        removeCollisions,
        trapDamage
    }

    public ZoneType zoneSelection;
    public int damageFromTrap;

    private void Start()
    {
        if (zoneSelection != ZoneType.trapDamage && GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }          
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            if (zoneSelection == ZoneType.ragdoll)
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
            else if (zoneSelection == ZoneType.both)
            {
                if (!other.gameObject.GetComponentInParent<PlayerController>().ragdolling)
                {
                    other.gameObject.GetComponentInParent<PlayerController>().Ragdoll(true);
                }

                other.gameObject.GetComponentInParent<PlayerController>().isDead = true;
                //other.gameObject.SetActive(false);
            }
            else if (zoneSelection == ZoneType.removeCollisions)
            {
                foreach (Collider collider in other.gameObject.GetComponentsInChildren<Collider>())
                {
                    //collider.enabled = false;
                }
            }

        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if (zoneSelection == ZoneType.trapDamage)
            {
                if (!collision.gameObject.GetComponentInParent<PlayerController>().ragdolling)
                {
                    collision.gameObject.GetComponentInParent<PlayerController>().Ragdoll(true);
                }
            }
        }
    }
}

////Custom inspector starts here
//#if UNITY_EDITOR

//[CustomEditor(typeof(KillZone))]
//public class KillzoneInspectorEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        //cast target
//        var killzoneScript = target as KillZone;

//        //Enum drop down
//        killzoneScript.zoneSelection = (KillZone.ZoneType)EditorGUILayout.EnumPopup(killzoneScript.zoneSelection);

//        switch (killzoneScript.zoneSelection)
//        {
//            //TrapDamage Zone
//            case KillZone.ZoneType.trapDamage:
//                killzoneScript.damageFromTrap = EditorGUILayout.IntField("Damage To Player", killzoneScript.damageFromTrap);
//                break;

//        }//end switch

//    }
//} //end KillzoneInspectorEditor

//#endif