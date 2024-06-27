using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private int currentIndex = 0;

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
    }

    private IEnumerator PlayCutscene()
    {
        while (true)
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
                    break;
                }
            }
        }
    }
}
