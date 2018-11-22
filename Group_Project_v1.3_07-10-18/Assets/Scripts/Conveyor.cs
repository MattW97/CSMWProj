using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public Transform endOfBelt;
    public float conveyorSpeed = 2;
    public float textureSpeed = 1;

    private void OnTriggerStay(Collider objectOnBelt)
    {
        if(objectOnBelt.tag != "Weapon")
        {
            objectOnBelt.transform.position = Vector3.MoveTowards(objectOnBelt.transform.position, endOfBelt.position, conveyorSpeed * Time.deltaTime);
        }

        if (objectOnBelt.tag == "Weapon")
        {
            if(objectOnBelt.transform.parent == null)
            {
                objectOnBelt.transform.position = Vector3.MoveTowards(objectOnBelt.transform.position, endOfBelt.position, conveyorSpeed * Time.deltaTime);
            }
        }
    }
}
