using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollToggle : MonoBehaviour {

    bool ragdoll = false;
	// Use this for initialization
	void Start () {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }

        GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<CapsuleCollider>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.Return) && !ragdoll)
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }
            GetComponent<Animator>().enabled = false;
            //GetComponent<CapsuleCollider>().enabled = false;

            ragdoll = true;
        }
        else if (Input.GetKeyDown(KeyCode.Return) && ragdoll)
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
            GetComponent<Animator>().enabled = true;
            //GetComponent<CapsuleCollider>().enabled = true;
            ragdoll = false;
        }
    }

    public void RunRagdoll()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
        }
        GetComponent<Animator>().enabled = false;
        ragdoll = true;
    }
}
