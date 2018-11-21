using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UtilityManager : MonoBehaviour
{
    void Start()
    {
        // Finds an object called New Game Object and destroys it
        // I don't know what spawns it - CS
        if (GameObject.Find("New Game Object") != null)
        {
            Destroy(GameObject.Find("New Game Object"));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKey(KeyCode.R))
        {
            // Reload current scene
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        } 
    }
}
