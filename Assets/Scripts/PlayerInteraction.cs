using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public CuttingBoardMiniGame cuttingBoardMiniGame;

    // Update is called once per frame
    void Update()
    {
        // Check if player presses 'E' and the cutting board mini-game is ready
        if (Input.GetKeyDown(KeyCode.E) && cuttingBoardMiniGame.IsReadyToStart())
        {
            Debug.Log("E");
            cuttingBoardMiniGame.StartMiniGame();
        }
    }
}
