using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

/// <summary>
/// This Script handles the player selection on the start screen
/// </summary>
public class PlayerPanel : MonoBehaviour {
       
    public int playerNumber;
    public bool hasControllerAssinged;
    public PlayerController player;
    public ControllerAssigner controlAssign;

    public GameObject join;
    public GameObject ready;
    public Color32 playerColour;

    private void Start()
    {
        join.SetActive(true);
        ready.SetActive(false);
        this.GetComponent<Image>().color = Color.white;
    }

    private void Update()
    {
        if (Input.GetButtonDown("B_" + playerNumber))
        {
            controlAssign.existingConNums.Remove(playerNumber);
            player.SetUpInputs(0);
            join.SetActive(true);
            ready.SetActive(false);
            this.GetComponent<Image>().color = Color.white;
            hasControllerAssinged = false;
        }
    }

    public PlayerController AssignController(int controller)
    {
        playerNumber = controller;
        StartCoroutine(Vibration());
        player.SetUpInputs(controller);
        join.SetActive(false);
        ready.SetActive(true);
        this.GetComponent<Image>().color = playerColour;
        hasControllerAssinged = true;
        return null;
    }

    /// <summary>
    /// "PlayerIndex" is either 0 - 4 with the actual controller repersented ben -1 to the value.
    /// i.e. PlayerIndex.Four = 3
    /// </summary>
    IEnumerator Vibration()
    {
        int tempPlayerNumber = playerNumber - 1;
        GamePad.SetVibration((PlayerIndex)tempPlayerNumber, 1, 1);
        yield return new WaitForSeconds(.1f);
        GamePad.SetVibration((PlayerIndex)tempPlayerNumber, 0, 0);
        yield break;
    }
}
