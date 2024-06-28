using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> cutsceneImages; // Lista de imágenes para la cutscene

    [SerializeField]
    private float interval = 2f; // Intervalo entre imágenes

    [SerializeField]
    private Image displayImage; // UI Image donde se mostrarán las imágenes

    [SerializeField]
    private bool loopCutscene = false; // Bandera para determinar si la cutscene se debe repetir

    private int currentIndex = 0;
    private bool canSkip = false; // Bandera para permitir saltar la cutscene
    private bool cutsceneActive = true; // Bandera para verificar si la cutscene está activa

    void Start()
    {
        if (cutsceneImages == null || cutsceneImages.Count == 0)
        {
            Debug.LogError("No se han asignado imágenes a la cutscene.");
            return;
        }

        if (displayImage == null)
        {
            Debug.LogError("No se ha asignado la referencia a la UI Image.");
            return;
        }

        StartCoroutine(PlayCutscene());
        StartCoroutine(EnableSkippingAfterDelay(3f)); // Permitir saltar después de 3 segundos
    }

    void Update()
    {
        if (cutsceneActive && canSkip && IsKeyboardInputReceived())
        {
            LoadMainMenu();
        }
    }

    private bool IsKeyboardInputReceived()
    {
        // Detecta entrada del teclado (excluye mouse)
        return Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2);
    }

    private IEnumerator PlayCutscene()
    {
        while (cutsceneActive)
        {
            displayImage.sprite = cutsceneImages[currentIndex];

            yield return new WaitForSeconds(interval);

            currentIndex++;

            if (currentIndex >= cutsceneImages.Count)
            {
                if (loopCutscene)
                {
                    currentIndex = 0;
                }
                else
                {
                    cutsceneActive = false;
                    break;
                }
            }
        }

        // Cambiar a la escena "Main Menu" cuando la cutscene haya terminado
        LoadMainMenu();
    }

    private IEnumerator EnableSkippingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canSkip = true; // Permitir saltar después del retraso
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
