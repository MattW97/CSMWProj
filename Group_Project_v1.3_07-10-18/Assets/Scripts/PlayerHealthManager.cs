using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour {

    public Image heartIcon;
    public List<Sprite> hearts;

    public float startingHealth;
    public float currentHealth;


    private void Start()
    {
        currentHealth = startingHealth;
    }

    private void Update()
    {

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (!GetComponent<PlayerController>().ragdolling)
            {
                GetComponent<PlayerController>().Ragdoll(true);
                heartIcon.enabled = false;
            }
        }

        if(currentHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }

        #region Hearts on UI
        ///This Just Changes the heart depending on health levels
        
        if (currentHealth >= 76) ///First Quater 76 - 100
        {
            heartIcon.enabled = true;
            heartIcon.sprite = hearts[0];
        }
        else if (currentHealth >= 51 && currentHealth <= 75) ///Second Quater 51 - 75
        {
            heartIcon.sprite = hearts[1];
        }
        else if (currentHealth >= 26 && currentHealth <= 50) ///Third Quater 26 - 50
        {
            heartIcon.sprite = hearts[2];
        }
        else if(currentHealth <= 25) ///Fourth Quater 0 - 25
        {
            heartIcon.sprite = hearts[3];
        }

        #endregion

    }

    public void DamagePlayer(int damageAmount)
    {
        currentHealth = currentHealth - damageAmount;
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth = currentHealth + healAmount;
    }
}
