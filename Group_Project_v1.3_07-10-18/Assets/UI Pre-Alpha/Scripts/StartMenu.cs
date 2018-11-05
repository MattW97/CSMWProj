using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the start menu UI nothing special just movin between menus,
/// Add access to options etc in here
/// </summary>
public class StartMenu : MonoBehaviour {

    [Header("Game Objects")]
    public GameObject startMenuGameObj;
    public GameObject playerSelectionGameObj;
    public GameObject startGameInfo;
    public GameObject parentObject;
    public GameObject overrideButton;
    private bool playerOverride;

    [Header("Links")]
    public List<PlayerController> players;
    public ControllerAssigner controlAssign;

    private bool inStart;

	void Start () {

        inStart = true;
        startMenuGameObj.SetActive(true);
        playerSelectionGameObj.SetActive(false);
        
        foreach (PlayerController player in players)
        {
            if (isActiveAndEnabled){

                player.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if(controlAssign.existingConNums.Count >= 2)
        {
            startGameInfo.SetActive(true);
        }
        else
        {
            startGameInfo.SetActive(false);
        }

        for (int i = 1; i <= 4; i++)
        {
            if (controlAssign.existingConNums.Count >= 2 && Input.GetButtonDown("Start_" + i))
            {
                parentObject.SetActive(false);

                foreach (PlayerController player in players)
                {

                    if (player.playerNum != 0)
                    {
                        player.PlayerInGame = true;

                        player.GetComponent<PlayerHealthManager>().heartIcon.gameObject.SetActive(true);

                        player.gameObject.SetActive(true);
                    }
                    else if (player.playerNum == 0)
                    {
                        player.PlayerInGame = false;
                    }
                }
            }
        }

        #region PlayerOverride

        if (controlAssign.existingConNums.Count >= 1)
        {
            overrideButton.SetActive(true);
        }
        else
        {
            overrideButton.SetActive(false);
        }
              

        if (playerOverride)
        {
            parentObject.SetActive(false);

            foreach (PlayerController player in players)
            {
                if (player.playerNum == 0)
                {
                    player.playerNum = 3;
                    player.PlayerInGame = true;

                    player.GetComponent<PlayerHealthManager>().heartIcon.gameObject.SetActive(true);

                    player.gameObject.SetActive(true);
                }
                else if (player.playerNum != 0)
                {
                    player.PlayerInGame = true;

                    player.GetComponent<PlayerHealthManager>().heartIcon.gameObject.SetActive(true);

                    player.gameObject.SetActive(true);
                }
            }
        }
        #endregion

    }

    public void RunOverride()
    {
        playerOverride = true;
    }

    public void OpenPlayerSelection()
    {
        startMenuGameObj.SetActive(false);
        playerSelectionGameObj.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
