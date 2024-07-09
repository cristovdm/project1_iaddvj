using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuCanvasManager : MonoBehaviour
{
    public GameObject pauseMenuCanvas; // El Canvas que contiene el men� de pausa

    void Start()
    {
        // Aseg�rate de que el men� de pausa est� oculto al inicio
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false); // Desactiva el Canvas al inicio
        }
        else
        {
            Debug.LogError("PauseMenuCanvas is not assigned.");
        }
    }

    // Pausar el juego y mostrar el men� de pausa
    public void PauseGame()
    {
        if (pauseMenuCanvas != null)
        {
            Debug.Log("Pausing Game");
            pauseMenuCanvas.SetActive(true); // Activa el Canvas
            Time.timeScale = 0f; // Pausa el juego
        }
    }

    // Reanudar el juego y ocultar el men� de pausa
    public void ResumeGame()
    {
        if (pauseMenuCanvas != null)
        {
            Debug.Log("Resuming Game");
            pauseMenuCanvas.SetActive(false); // Desactiva el Canvas
            Time.timeScale = 1f; // Reanuda el juego
        }
    }

    // Volver al men� principal
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Aseg�rate de que el juego no sigue pausado
        SceneManager.LoadScene("Main Menu");
    }
}
