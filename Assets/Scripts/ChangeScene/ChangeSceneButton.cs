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
            Level.Instance.SetLevel(Day.Instance.GetCurrentDay(),1);
            SceneManager.LoadScene("Maze");
        }
        else if (currentScene.name == "Maze")
        {
            Day.Instance.NextDay();
            Level.Instance.SetLevel(Day.Instance.GetCurrentDay(), 2);
            SceneManager.LoadScene("Kitchen");
        }

        canvasChangeScene.gameObject.SetActive(false);
    }
}