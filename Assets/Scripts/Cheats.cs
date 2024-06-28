using System;
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

    private Dictionary<string, Action> cheatCodes = new Dictionary<string, Action>();
    private Dictionary<string, int> cheatCodeIndices = new Dictionary<string, int>();
    private AudioSource audioSource;
    private Image canvasImage;
    private TMP_Text canvasText;
    private bool isFading = false;
    private InventoryController trashInventory;
    private Money moneyScript;

    [SerializeField] private InventorySO trashInventoryData;

    private List<string> rottenItemNames = new List<string>
    {
        "RottenEgg",
        "RottenCorn",
        "RottenTomato",
        "RottenCarrot",
        "RottenFish",
        "RottenPan"
    };

    private void Start()
    {
        trashInventory = FindObjectOfType<InventoryController>();
        moneyScript = FindObjectOfType<Money>();

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

    private void Update()
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

    private void ActivateHESOYAM()
    {
        fillMyTrash();
        moneyScript.AddMoney(100);
        ActivateCheatCanvas();
    }

    private void ActivateGOAWAY()
    {
        DestroyTargetObject();
        ActivateCheatCanvas();
    }

    private void ActivateCheatCanvas()
    {
        StopAllCoroutines();
        cheatCanvas.gameObject.SetActive(true);
        ResetCanvasAlpha();
        PlayCheatSound();
        StartCoroutine(FadeOutCanvas());
    }

    private void PlayCheatSound()
    {
        if (cheatSound != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(cheatSound);
        }
    }

    private void DestroyTargetObject()
    {
        List<string> targetNames = new List<string> { "RataSucia(Clone)", "seaurchin(Clone)", "BananaOtter(Clone)" };

        foreach (string targetName in targetNames)
        {
            GameObject targetObject = GameObject.Find(targetName);
            if (targetObject != null)
            {
                Destroy(targetObject);
            }
        }
    }

    private void fillMyTrash()
    {
        
        int index = 0;

        foreach (string itemName in rottenItemNames)
        {
            try
            {
                InventoryItem inventoryItem = trashInventoryData.GetItemAt(index);

                if (!inventoryItem.IsEmpty)
                {
                    trashInventory.DropTrashItem(index, inventoryItem.quantity);
                }
            }
            catch (Exception ex)
            {
            }

            EdibleItemSO itemSO = ResourceManager.LoadResource<EdibleItemSO>(itemName);
            if (itemSO != null)
            {
                InventoryItem item = new InventoryItem
                {
                    item = itemSO,
                    quantity = 9,
                    itemState = new List<ItemParameter>()
                };
                trashInventory.AddTrashInventoryItem(item);

                index++;
            }
            else
            {
                Debug.LogWarning($"No se pudo cargar el ítem {itemName}.");
            }
        }
    }

    private IEnumerator FadeOutCanvas()
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

    private void ResetCanvasAlpha()
    {
        Color canvasColor = canvasImage.color;
        canvasColor.a = 1f;
        canvasImage.color = canvasColor;

        Color textColor = canvasText.color;
        textColor.a = 1f;
        canvasText.color = textColor;
    }
}
