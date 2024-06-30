using UnityEngine;
using System.Linq; // Ensure you have this using directive for Linq methods

public class SceneInitializer : MonoBehaviour
{
    private GameObject canvasGameObject;

    void Start()
    {
        canvasGameObject = GameObject.Find("Canvas");

        if (canvasGameObject != null)
        {
            ResetMoneyAndDebt();
            ResetDay();
            ResetLevel();
        }
        else
        {
            Debug.LogError("Canvas GameObject named 'Canvas' not found in the scene."); 
        }
    }

    private void ResetMoneyAndDebt()
    {
        Money moneyManager = canvasGameObject.GetComponentInChildren<Money>(true);
        if (moneyManager != null)
        {
            moneyManager.ResetMoneyAndDebt();
            Debug.Log("Money and debt reset successfully.");
        }
        else
        {
            Debug.LogError("Money component not found in the children of the Canvas GameObject.");
        }
    }

    private void ResetDay()
    {
        Day dayManager = canvasGameObject.GetComponentInChildren<Day>(true);
        if (dayManager != null)
        {
            dayManager.ResetDay();
            Debug.Log("Day reset successfully.");
        }
        else
        {
            Debug.LogError("Day component not found in the children of the Canvas GameObject.");
        }
    }

    private void ResetLevel()
    {
        Level levelManager = canvasGameObject.GetComponentInChildren<Level>(true);
        if (levelManager != null)
        {
            levelManager.ResetLevel();
            Debug.Log("Level reset successfully.");
        }
        else
        {
            Debug.LogError("Level component not found in the children of the Canvas GameObject.");
        }
    }
}