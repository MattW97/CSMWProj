using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour {

    public Image heartIcon;
    public List<Sprite> hearts;

    public float startingHealth;
    private float currentHealth;


    private void Start()
    {
        CurrentHealth = startingHealth;
    }

    private void Update()
    {

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            if (!GetComponent<PlayerController>().ragdolling)
            {
                GetComponent<PlayerController>().Ragdoll(true);
                heartIcon.enabled = false;
            }
        }

        if(CurrentHealth > startingHealth)
        {
            CurrentHealth = startingHealth;
        }

        #region Hearts on UI
        ///This Just Changes the heart depending on health levels
        
        if (CurrentHealth >= 76) ///First Quater 76 - 100
        {
            heartIcon.enabled = true;
            heartIcon.sprite = hearts[0];
        }
        else if (CurrentHealth >= 51 && CurrentHealth <= 75) ///Second Quater 51 - 75
        {
            heartIcon.sprite = hearts[1];
        }
        else if (CurrentHealth >= 26 && CurrentHealth <= 50) ///Third Quater 26 - 50
        {
            heartIcon.sprite = hearts[2];
        }
        else if(CurrentHealth <= 25) ///Fourth Quater 0 - 25
        {
            heartIcon.sprite = hearts[3];
        }

        #endregion

    }

    public void DamagePlayer(int damageAmount)
    {
        CurrentHealth = CurrentHealth - damageAmount;
    }

    public void HealPlayer(int healAmount)
    {
        CurrentHealth = CurrentHealth + healAmount;
    }

    #region Getters/ Setters

    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    #endregion
}
