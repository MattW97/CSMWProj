using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour {

    public Image healthBar;
    public Text healthValue;

    public float startingHealth;
    public float currentHealth;

    private void Start() {

        currentHealth = startingHealth;
    }

    private void Update() {

        if (currentHealth <= 0) {
            // Death code, maybe ragdoll?
            // Placeholder for now:
            currentHealth = 0;
            //this.GetComponent<PlayerController>().isDead = true;
            if(!GetComponent<PlayerController>().ragdolling)
                GetComponent<PlayerController>().Ragdoll(true);
        }

        if(currentHealth > startingHealth) {

            currentHealth = startingHealth;
        }

        healthValue.text = currentHealth.ToString();
        healthBar.fillAmount = currentHealth / startingHealth;
    }

    public void DamagePlayer(int damageAmount) {

        currentHealth = currentHealth - damageAmount;
    }

    public void HealPlayer(int healAmount) {

        currentHealth = currentHealth + healAmount;
    }

}
