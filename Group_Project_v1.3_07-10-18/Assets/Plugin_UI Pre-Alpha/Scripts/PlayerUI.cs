using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This now handles the UI for the player, Everything apart from Health is handled within this script.
/// health is still in Player Health Manager
/// </summary>
public class PlayerUI : MonoBehaviour {
    
    public Image yMash;                                         // Y Button
    public Image portrait;                                      // Portait image
    public Image shellIcon;                                     // Where the shell (Ammo) icon is replaced
    public List<Sprite> shells;                                 // The sprites that replace the ammo
    public RuntimeAnimatorController animatorControl;           // THis is passed in so that the ammo animation resets

    public PlayerController assignedPlayer;                     // Link to assinged player ( UI 1 goes to Player 1 )

    void Update()
    {
        yMash.fillAmount = assignedPlayer.totalCurrentMashes / assignedPlayer.numToMash;

        #region Ammo Shells on UI

        //if (assignedPlayer.InitAmmoAmount == 4)
        //{
        //    shellIcon.GetComponent<Animator>().runtimeAnimatorController = null;
        //    shellIcon.sprite = shells[0];
        //}
        //else if (assignedPlayer.InitAmmoAmount == 3)
        //{
        //    shellIcon.sprite = shells[1];
        //}
        //else if (assignedPlayer.InitAmmoAmount == 2)
        //{
        //    shellIcon.sprite = shells[2];
        //}
        //else if (assignedPlayer.InitAmmoAmount == 1)
        //{
        //    shellIcon.sprite = shells[3];
        //}
        //else if (assignedPlayer.InitAmmoAmount == 0)
        //{
        //    shellIcon.sprite = shells[4];
        //    shellIcon.GetComponent<Animator>().runtimeAnimatorController = animatorControl;
        //    shellIcon.GetComponent<Animator>().SetBool("play", true);
        //}

        #endregion

        if (assignedPlayer.ragdolling)
        {
            yMash.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            yMash.transform.parent.gameObject.SetActive(false);
        }

        if (!assignedPlayer.PlayerInGame)
        {
            portrait.gameObject.SetActive(false);
            shellIcon.gameObject.SetActive(false);
            yMash.transform.parent.gameObject.SetActive(false);
            assignedPlayer.GetComponent<PlayerHealthManager>().heartIcon.gameObject.SetActive(false);
        }
        else
        {
            portrait.gameObject.SetActive(true);
            shellIcon.gameObject.SetActive(true);
            assignedPlayer.GetComponent<PlayerHealthManager>().heartIcon.gameObject.SetActive(true);

        }
    }
}
