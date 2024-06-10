using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public Canvas canvasChangeScene;

    public void ChangeScene()
    {
        Scene currentScene = SceneManager.GetActiveScene(); 
        Debug.Log("Current scene: " + currentScene.name); 

        if (currentScene.name == "Kitchen")
        {
            SceneManager.LoadScene("Maze");
            Day.Instance.NextDay();
        }
        else if (currentScene.name == "Maze")
        {
            SceneManager.LoadScene("Kitchen");
        }


        canvasChangeScene.gameObject.SetActive(false);
    }
}