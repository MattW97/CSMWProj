using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour {

    public GameObject conveyor;
    public Transform endOfBelt;
    public float conveyorSpeed = 2;
    public float textureSpeed = 1;

    private void OnTriggerStay(Collider objectOnBelt)
    {
        objectOnBelt.transform.position = Vector3.MoveTowards(objectOnBelt.transform.position, endOfBelt.position, conveyorSpeed * Time.deltaTime);
    }

    //private void Update()
    //{        
    //    float texOffset = Time.time * textureSpeed;
    //    GetComponent<MeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0, texOffset));
    //}
}
