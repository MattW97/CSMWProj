using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeScript : MonoBehaviour {

    public Canvas playerWinCanvas;
    public Text playerWinText;

    public GameObject player1;
    public GameObject player2;

    // Use this for initialization
    void Start () {

        playerWinCanvas.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
        if(player1.GetComponent<PlayerController>().isDead)
        {
            // Player 2 wins
            Debug.Log("Player 2 Wins!");
            player2.GetComponent<PlayerController>().canControl = false;

            playerWinCanvas.enabled = true;
            playerWinText.text = "Player 2 Wins!";

        }

        if (player2.GetComponent<PlayerController>().isDead)
        {
            // Player 1 wins
            Debug.Log("Player 1 Wins!");
            player1.GetComponent<PlayerController>().canControl = false;

            playerWinCanvas.enabled = true;
            playerWinText.text = "Player 1 Wins!";
        }
    }
}
