using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public void PlayGame()
    {
        Scene currentScene = SceneManager.GetActiveScene(); 
        Debug.Log("Current scene: " + currentScene.name); 

        if (currentScene.name == "Kitchen")
        {
            SceneManager.LoadScene("Maze");
        }
        else if (currentScene.name == "Maze")
        {
            SceneManager.LoadScene("Kitchen");
        }
    }
}