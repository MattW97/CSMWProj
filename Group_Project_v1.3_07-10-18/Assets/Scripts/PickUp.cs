using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickUp : MonoBehaviour {

    public float force = 4000;
    public Rigidbody rb;
    public bool join = false; public GameObject playerToJoinTo;
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
            playerToJoinTo = GetComponentInParent<PlayerController>().ClosestPlayer;
            sp.connectedBody = playerToJoinTo.GetComponentInParent<PlayerController>().rightHand;
            ConnectionPoint = sp.connectedBody;
            sp.autoConfigureConnectedAnchor = false;
            sp.connectedAnchor = new Vector3(0, 0, 0);

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
