using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> cutsceneImages;

    [SerializeField]
    private float interval = 2f;

    [SerializeField]
    private Image displayImage;

    [SerializeField]
    private bool loopCutscene = false;

    [SerializeField]
    private Image fadeImage;

    [SerializeField]
    private float fadeDuration = 2f;

    [SerializeField]
    private AudioSource backgroundMusic;

    private int currentIndex = 0;
    private bool canSkip = false;
    private bool cutsceneActive = true;

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

        if (fadeImage == null)
        {
            Debug.LogError("No se ha asignado la referencia a la Image de fade.");
            return;
        }

        if (backgroundMusic == null)
        {
            Debug.LogError("No se ha asignado la referencia al AudioSource de música.");
            return;
        }

        StartCoroutine(PlayCutscene());
        StartCoroutine(EnableSkippingAfterDelay(3f));
    }

    void Update()
    {
        if (cutsceneActive && canSkip && IsKeyboardInputReceived())
        {
            StartCoroutine(FadeToBlackAndLoadMainMenu());
        }
    }

    private bool IsKeyboardInputReceived()
    {
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

        StartCoroutine(FadeToBlackAndLoadMainMenu());
    }

    private IEnumerator EnableSkippingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canSkip = true;
    }

    private IEnumerator FadeToBlackAndLoadMainMenu()
    {
        float startVolume = backgroundMusic.volume;

        fadeImage.color = new Color(0, 0, 0, 0);

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            backgroundMusic.volume = startVolume * (1 - alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1);
        backgroundMusic.volume = 0;

        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
