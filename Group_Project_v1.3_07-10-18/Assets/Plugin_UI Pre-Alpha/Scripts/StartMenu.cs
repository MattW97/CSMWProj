using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Handles the start menu UI nothing special just movin between menus,
/// Add access to options etc in here
/// </summary>
public class StartMenu : MonoBehaviour
{

    [Header("Game Objects")]
    public GameObject startMenuGameObj;
    public GameObject creditMenuGameObj;
    public GameObject optionMenuGameObj;
    public GameObject playerSelectionGameObj;
    public GameObject startGameInfo;
    public GameObject parentObject;
    public GameObject overrideButton;
    public GameObject fightButton;
    public GameObject closeOptionsButton;

    public List<Sprite> countdownSprites;
    private int spritenumber;
    public GameObject countdownScreen;
    public Image countdownPosition;

    public Camera mainCamera;
    public Camera menuFlyCamera;
    public Camera countdownCamera;

    private bool playerOverride;
    private EventSystem es;

    [Header("Links")]
    public List<PlayerController> players;
    public ControllerAssigner controlAssign;
    private List<PlayerController> playersInGame;

    //private bool inStart;

    void Start()
    {
        mainCamera.rect = new Rect(0, 0, 0, 0);

        playersInGame = new List<PlayerController>();

        es = EventSystem.current;
        es.SetSelectedGameObject(fightButton);

        menuFlyCamera.gameObject.SetActive(true);
        countdownCamera.gameObject.SetActive(false);

        //inStart = true;

        startMenuGameObj.SetActive(true);
        startGameInfo.SetActive(false);
        playerSelectionGameObj.SetActive(false);
        optionMenuGameObj.SetActive(false);
        creditMenuGameObj.SetActive(false);
        countdownScreen.SetActive(false);

        foreach (PlayerController player in players)
        {
            if (isActiveAndEnabled)
            {

                player.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        #region Check For Player joined On Start Menu

        if (controlAssign.existingConNums.Count >= 2)
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
                //parentObject.SetActive(false);
                menuFlyCamera.gameObject.SetActive(false);
                playerSelectionGameObj.SetActive(false);

                //Begin Countdown Here
                StartCoroutine(Countdown());

                foreach (PlayerController player in players)
                {

                    if (player.playerNum != 0)
                    {
                        player.PlayerInGame = true;
                        player.canControl = false;

                        if (!playersInGame.Contains(player))
                            playersInGame.Add(player);

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

        #endregion

        #region Back To Start

        if (Input.GetButtonDown("B_1") || Input.GetButtonDown("B_2") || Input.GetButtonDown("B_3") || Input.GetButtonDown("B_4"))
        {
            startMenuGameObj.SetActive(true);
            playerSelectionGameObj.SetActive(false);
            optionMenuGameObj.SetActive(false);
            creditMenuGameObj.SetActive(false);

            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(fightButton);
        }

        #endregion

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
            playerSelectionGameObj.SetActive(false);
            //parentObject.SetActive(false);
            HideAll();

            foreach (PlayerController player in players)
            {
                player.inCountdown = true;

                if (player.playerNum == 0)
                {
                    player.playerNum = 3;
                    player.PlayerInGame = true;

                    player.GetComponent<PlayerHealthManager>().heartIcon.gameObject.SetActive(true);

                    player.canControl = false;

                    if (!playersInGame.Contains(player))
                        playersInGame.Add(player);

                    player.gameObject.SetActive(true);
                }
                else if (player.playerNum != 0)
                {
                    player.PlayerInGame = true;

                    player.canControl = false;

                    if (!playersInGame.Contains(player))
                        playersInGame.Add(player);


                    player.GetComponent<PlayerHealthManager>().heartIcon.gameObject.SetActive(true);

                    player.gameObject.SetActive(true);
                }
            }
        }
        #endregion

    }

    IEnumerator Countdown()
    {
        menuFlyCamera.gameObject.SetActive(false);
        countdownCamera.gameObject.SetActive(true);
        countdownScreen.SetActive(true);

        countdownPosition.sprite = countdownSprites[spritenumber];

        yield return new WaitForSeconds(1);

        spritenumber++;

        if (spritenumber != countdownSprites.Count)
        {
            StartCoroutine(Countdown());
        }
        else
        {
            foreach (PlayerController player in playersInGame)
            {
                player.canControl = true;
                player.inCountdown = false;
            }

            //countdownScreen.SetActive(false);
            menuFlyCamera.gameObject.SetActive(false);
            countdownCamera.gameObject.SetActive(false);
            mainCamera.rect = new Rect(0, 0, 1, 1);
            parentObject.SetActive(false);
        }
    }

    public void RunOverride()
    {
        playerOverride = true;
        menuFlyCamera.gameObject.SetActive(false);
        StartCoroutine(Countdown());
    }

    public void OpenPlayerSelection()
    {
        startMenuGameObj.SetActive(false);
        playerSelectionGameObj.SetActive(true);
    }

    public void Options()
    {
        startMenuGameObj.SetActive(false);
        optionMenuGameObj.SetActive(true);


        es.SetSelectedGameObject(null);
        es.SetSelectedGameObject(closeOptionsButton);
    }

    public void Credits()
    {
        startMenuGameObj.SetActive(false);
        creditMenuGameObj.SetActive(true);
    }

    public void CloseOptions()
    {
        startMenuGameObj.SetActive(true);
        optionMenuGameObj.SetActive(false);

        es.SetSelectedGameObject(null);
        es.SetSelectedGameObject(fightButton);
    }

    private void HideAll()
    {
        playerSelectionGameObj.SetActive(false);
        menuFlyCamera.gameObject.SetActive(false);
        startMenuGameObj.SetActive(false);
        startGameInfo.SetActive(false);
        optionMenuGameObj.SetActive(false);
        creditMenuGameObj.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
