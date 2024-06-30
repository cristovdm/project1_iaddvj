using UnityEngine;
using System.Linq; // Ensure you have this using directive for Linq methods

public class SceneInitializer : MonoBehaviour
{
    void Start()
    {
        // Find the Canvas GameObject by name
        GameObject canvasGameObject = GameObject.Find("Canvas");
        if (canvasGameObject != null)
        {
            // Try to find the Money component in the children of the Canvas GameObject, including inactive ones
            Money moneyManager = canvasGameObject.GetComponentInChildren<Money>(true); // true to include inactive GameObjects
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
        else
        {
            Debug.LogError("Canvas GameObject named 'Canvas' not found in the scene."); // Corrected the case to match the search
        }
    }
}