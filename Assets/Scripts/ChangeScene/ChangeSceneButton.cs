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
            Level.Instance.SetLevel(Day.Instance.GetCurrentDay(),1);
        }
        else if (currentScene.name == "Maze")
        {
            Day.Instance.NextDay();
            SceneManager.LoadScene("Kitchen");
            Level.Instance.SetLevel(Day.Instance.GetCurrentDay(), 2);
        }

        canvasChangeScene.gameObject.SetActive(false);
    }
}