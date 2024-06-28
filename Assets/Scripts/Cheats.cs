using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory;
using Inventory.Model;

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
    private InventoryController trashInventory; 
    private ItemSO cleanedItem;
    private Money moneyScript;
    private Level levelScript; 

    void Start()
    {
        trashInventory = FindObjectOfType<InventoryController>();
        moneyScript = FindObjectOfType<Money>();
        levelScript = FindObjectOfType<Level>();

        if (levelScript != null)
        {
            string currentLevelName = levelScript.GetCurrentLevel();
            Debug.Log("Current Level: " + currentLevelName);
        }
        else
        {
            Debug.LogWarning("Level script not found!");
        }

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
        //fillMyTrash(); 
        moneyScript.AddMoney(100);
        ActivateCheatCanvas();
    }

    void ActivateGOAWAY()
    {
        DestroyTargetObject();
        ActivateCheatCanvas();
    }

    void ActivateCheatCanvas()
    {
        StopAllCoroutines();
        cheatCanvas.gameObject.SetActive(true);
        ResetCanvasAlpha();
        PlayCheatSound();
        StartCoroutine(FadeOutCanvas());
    }

    void PlayCheatSound()
    {
        if (cheatSound != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(cheatSound);
        }
    }

    void DestroyTargetObject()
    {
        List<string> targetNames = new List<string> { "RataSucia(Clone)", "seaurchin(Clone)", "BananaOtter(Clone)" };

        foreach (string targetName in targetNames)
        {
            GameObject targetObject = GameObject.Find(targetName);
            if (targetObject != null)
            {
                Destroy(targetObject);
                Debug.Log($"{targetName} destroyed");
            }
        }
    }

    void fillMyTrash(){ 
        cleanedItem = ResourceManager.LoadResource<EdibleItemSO>("Carrot");
        InventoryItem item = new InventoryItem
        {
            item = cleanedItem,
            quantity = 1,
            itemState = new List<ItemParameter>()
        };
        trashInventory.AddTrashInventoryItem(item);

    }

    IEnumerator FadeOutCanvas()
    {
        yield return new WaitForSeconds(1.6f);

        isFading = true;
        float fadeDuration = 2f;
        float fadeStep = 0.05f;
        Color canvasColor = canvasImage.color;
        Color textColor = canvasText.color;

        for (float t = 0; t < fadeDuration; t += fadeStep)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            canvasColor.a = alpha;
            textColor.a = alpha;
            canvasImage.color = canvasColor;
            canvasText.color = textColor;
            yield return new WaitForSeconds(fadeStep);
        }

        cheatCanvas.gameObject.SetActive(false);
        isFading = false;
    }

    void ResetCanvasAlpha()
    {
        Color canvasColor = canvasImage.color;
        canvasColor.a = 1f;
        canvasImage.color = canvasColor;

        Color textColor = canvasText.color;
        textColor.a = 1f;
        canvasText.color = textColor;
    }
}
