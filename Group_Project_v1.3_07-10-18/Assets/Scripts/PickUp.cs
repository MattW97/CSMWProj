using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickUp : MonoBehaviour {

    public float force = 4000;
    public Rigidbody rb;
    public bool join = false;
    private Rigidbody connectionPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void CreateJoint()
    {
        if (!join)
        {
            SpringJoint sp = gameObject.AddComponent<SpringJoint>();
            sp.connectedBody = GetComponentInParent<PlayerController>().pointToGrab;
            ConnectionPoint = sp.connectedBody;
            //ConnectionPoint.GetComponentInParent<PlayerController>().beenDragged = true;
            //ConnectionPoint.GetComponentInParent<PlayerController>().TotalCurrentMashes = 0;
            sp.autoConfigureConnectedAnchor = false;
            //sp.connectedAnchor = connectionPoint;

            sp.spring = 12000;
            sp.enableCollision = true;

            join = true;
        }
    }

    #region Getters/ Setters

    public Rigidbody ConnectionPoint
    {
        get
        {
            return connectionPoint;
        }

        set
        {
            connectionPoint = value;
        }
    }
    #endregion
}
