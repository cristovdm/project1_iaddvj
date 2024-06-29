using Inventory.Model;
using Inventory;
using System.Collections;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class FryerMinigame : MonoBehaviour
{
    public GameObject ParentObject;
    public BoxCollider2D interactionArea;
    public TextMeshProUGUI feedbackText;
    public GameObject targetSprite;
    public List<GameObject> additionalSprites;
    public AudioClip activationClip;
    public AudioClip errorSound;
    public AudioSource audioSource;

    public PlayerMovement playerMovement;
    private InventoryController inventory;
    private ItemSO cutItem;

    private float targetTime;
    private float elapsedTime;
    private bool isPressingKey;
    private bool gameStarted;
    private const float marginOfError = 1f;

    private bool isCooldown = false;
    private bool gameActive = false;
    private bool isPlayerLocked = true;
    private bool win = false;
    private bool readyToStart = true;
    private bool hasStartedMiniGame = false;

    void Start()
    {
        inventory = FindObjectOfType<InventoryController>();
        SetChildrenActive(ParentObject, false);
    }

    void Update()
    {
        if (isCooldown)
        {
            return;
        }

        if (gameActive)
        {
            playerMovement.enabled = false;
        }
        else
        {
            playerMovement.enabled = true;
        }

        if (gameActive && !isPlayerLocked && !win)
        {
            if (gameStarted)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    audioSource.PlayOneShot(activationClip);
                    isPressingKey = true;
                    elapsedTime = 0f;
                    feedbackText.gameObject.SetActive(false);
                    audioSource.loop = true;
                    audioSource.Play();
                }

                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    if (isPressingKey)
                    {
                        isPressingKey = false;
                        audioSource.Stop();

                        if (elapsedTime >= targetTime)
                        {
                            DeactivateAllAdditionalSprites();
                            feedbackText.gameObject.SetActive(true);
                            feedbackText.text = "COMPLETE!";
                            Invoke("EndMiniGame", 1.0f);
                        }
                        else
                        {
                            feedbackText.gameObject.SetActive(true);
                            feedbackText.text = "Try again";
                            StartCoroutine(RestartGameAfterDelay(2f));
                        }
                    }
                }

                if (isPressingKey)
                {
                    elapsedTime += Time.deltaTime;
                    UpdateSprites();
                }
            }
        }
        
        {
            if (!IsGameActive() && !hasStartedMiniGame && Input.GetKeyDown(KeyCode.E) && IsReadyToStart() && IsFoodAvailable())
            {
                StartMiniGame();
            }
        }
    }

    void SetChildrenActive(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

    void StartGame()
    {
        targetTime = Random.Range(2f, 5f);
        elapsedTime = 0f;
        isPressingKey = false;
        gameStarted = true;

        feedbackText.gameObject.SetActive(true);
        feedbackText.text = "Hold until you see 5 bubbles.";
        targetSprite.SetActive(true);
        DeactivateAllAdditionalSprites();
    }

    void DeactivateAllAdditionalSprites()
    {
        foreach (var sprite in additionalSprites)
        {
            sprite.SetActive(false);
        }
    }

    void UpdateSprites()
    {
        float timePerSprite = targetTime / additionalSprites.Count;
        for (int i = 0; i < additionalSprites.Count; i++)
        {
            if (elapsedTime >= timePerSprite * (i + 1))
            {
                additionalSprites[i].SetActive(true);
            }
        }
    }

    IEnumerator RestartGameAfterDelay(float delay)
    {
        gameStarted = false;
        yield return new WaitForSeconds(delay);
        StartGame();
    }

    public void StartMiniGame()
    {
        feedbackText.text = "";
        inventory.TakeOutFirstItem();
        hasStartedMiniGame = true;
        SetChildrenActive(ParentObject, true);
        targetSprite.SetActive(true);

        gameActive = true;
        isPlayerLocked = false;
        StartGame();
    }

    private void ResetGame()
    {
        DeactivateAllAdditionalSprites();
        hasStartedMiniGame = false;
        gameActive = false;
        isPlayerLocked = true;
        readyToStart = false;
        win = false;
    }

    void StartCooldown()
    {
        isCooldown = true;
        StartCoroutine(ResetCooldown());
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(3f);
        isCooldown = false;
    }

    void EndMiniGame()
    {
        StopAllCoroutines();
        ResetGame();
        SetChildrenActive(ParentObject, false);
        StartCooldown();
        playerMovement.enabled = true;

        InventoryItem item = new InventoryItem
        {
            item = cutItem,
            quantity = 1,
            itemState = new List<ItemParameter>()
        };
        inventory.AddInventoryItem(item);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            readyToStart = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            readyToStart = false;
        }
    }

    public bool IsReadyToStart()
    {
        if (interactionArea == null)
        {
            Debug.LogError("Interaction Area has not been assigned in the inspector!");
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll(interactionArea.bounds.center, interactionArea.bounds.size, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsFoodAvailable()
    {
        InventoryItem inventoryItem = inventory.GetInventoryFirstItem();

        if (!inventoryItem.IsEmpty)
        {
            Debug.Log(inventoryItem.item.Name);
            switch (inventoryItem.item.Name)
            {
                case "Egg":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("FriedEgg");
                    return true;

                case "PanCortado":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("PanCortadoFrito");
                    return true;

                case "Fish":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("FriedFish");
                    return true;

                case "CutCorn":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("PopCorn");
                    return true;
            }
            return false;
        }
        else {
        
            audioSource.PlayOneShot(errorSound);
            return false;
        }
        
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}
