using UnityEngine;

public class PauseMenuInputManager : MonoBehaviour
{
    public PauseMenuCanvasManager canvasManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (canvasManager != null)
            {
                if (canvasManager.pauseMenuCanvas.activeSelf)
                {
                    canvasManager.ResumeGame();
                }
                else
                {
                    canvasManager.PauseGame();
                }
            }
        }
    }
}
