using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheatCodeListener : MonoBehaviour
{
    public Canvas cheatCanvas;
    public AudioClip cheatSound;

    private Dictionary<string, System.Action> cheatCodes = new Dictionary<string, System.Action>();
    private Dictionary<string, int> cheatCodeIndices = new Dictionary<string, int>();
    private AudioSource audioSource;
    private Image canvasImage;
    private TMP_Text canvasText;
    private bool isFading = false;

    void Start()
    {
        cheatCodes.Add("HESOYAM", ActivateHESOYAM);
        cheatCodes.Add("GOAWAY", ActivateGOAWAY);

        foreach (var code in cheatCodes.Keys)
        {
            cheatCodeIndices[code] = 0;
        }

        audioSource = GetComponent<AudioSource>();
        canvasImage = cheatCanvas.GetComponentInChildren<Image>();
        canvasText = canvasImage.GetComponentInChildren<TMP_Text>();
        cheatCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.anyKeyDown && !isFading)
        {
            foreach (char c in Input.inputString)
            {
                foreach (var code in cheatCodes.Keys)
                {
                    if (char.ToUpper(c) == code[cheatCodeIndices[code]])
                    {
                        cheatCodeIndices[code]++;
                        if (cheatCodeIndices[code] == code.Length)
                        {
                            cheatCodes[code].Invoke();
                            cheatCodeIndices[code] = 0; 
                        }
                    }
                    else
                    {
                        cheatCodeIndices[code] = 0; 
                    }
                }
            }
        }
    }

    void ActivateHESOYAM()
    {
        Debug.Log("Truco HESOYAM activado");
        ActivateCheatCanvas();
    }

    void ActivateGOAWAY()
    {
        Debug.Log("Truco GOAWAY activado");
        ActivateCheatCanvas();
    }

    void ActivateCheatCanvas()
    {
        cheatCanvas.gameObject.SetActive(true);
        StartCoroutine(FadeOutCanvas());
        PlayCheatSound();
    }

    void PlayCheatSound()
    {
        if (cheatSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(cheatSound);
        }
    }

    IEnumerator FadeOutCanvas()
    {
        yield return new WaitForSeconds(1.2f);

        isFading = true;
        float fadeDuration = 2f;
        float fadeStep = 0.05f;
        Color canvasColor = canvasImage.color;
        Color textColor = canvasText.color;

        for (float t = 0; t < fadeDuration; t += fadeStep)
        {
            float alpha = Mathf.Lerp(0.7f, 0f, t / fadeDuration);
            canvasColor.a = alpha;
            textColor.a = alpha;
            canvasImage.color = canvasColor;
            canvasText.color = textColor;
            yield return new WaitForSeconds(fadeStep);
        }

        cheatCanvas.gameObject.SetActive(false);
        isFading = false;
    }
}
