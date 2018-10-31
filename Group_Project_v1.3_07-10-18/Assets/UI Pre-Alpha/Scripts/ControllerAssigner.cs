using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///  This script handles player assigning to avaliable positions within the game.
///  So player will get given which ever is the lowest avaliable player position.
/// </summary>
public class ControllerAssigner : MonoBehaviour {

    public PlayerPanel[] playerPanels;
    public List<int> existingConNums;

    private void Start()
    {
        existingConNums.Clear();
    }

    private void Update()
    {
        for (int i = 1; i <= 4; i++)
        {
            if (Input.GetButtonDown("A_" + i) && !existingConNums.Contains(i))
            {
                existingConNums.Add(i);
                AssignPlayer(i);
            }
        }
    }

    public PlayerController AssignPlayer(int controller)
    {
        for (int i = 0; i < playerPanels.Length; i++)
        {
            if (playerPanels[i].hasControllerAssinged == false)
            {
                return playerPanels[i].AssignController(controller);
            }
        }
        return null;
    }
}
