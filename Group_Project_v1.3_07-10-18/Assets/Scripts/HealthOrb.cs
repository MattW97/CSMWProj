using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour {

    public bool canHeal;
    private float timer;
    private float canHealTime;

    void Start () {

        canHeal = true;
        canHealTime = 5;
        timer = canHealTime;
    }

    void Update () {

        if(!canHeal) {

            timer -= Time.deltaTime;
            this.GetComponent<MeshRenderer>().enabled = false;
        }

        if(timer <= 0) {

            canHeal = true;
            timer = canHealTime;
            this.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void OnTriggerEnter(Collider other) {

        if(canHeal) {

            if (other.gameObject.tag == "Player") {

                if (other.gameObject.GetComponent<PlayerHealthManager>().currentHealth < other.gameObject.GetComponent<PlayerHealthManager>().startingHealth) {

                    other.gameObject.GetComponent<PlayerHealthManager>().HealPlayer(25);
                    canHeal = false;
                }
            }
        }      
    }
}
