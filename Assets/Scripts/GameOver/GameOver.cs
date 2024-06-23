using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void BackToMenu()
    {
        Debug.Log("Back to menu");
        SceneManager.LoadScene("Main Menu");
    }
}
