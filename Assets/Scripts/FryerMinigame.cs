using Inventory.Model;
using Inventory;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField] private Canvas messageCanvas;
    [SerializeField] private Image informationImage;
    [SerializeField] private Image arrowImage;
    [SerializeField] private TextMeshProUGUI messageText;
    

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
        audioSource = GetComponent<AudioSource>();
        SetChildrenActive(ParentObject, false);
        informationImage.enabled = false; 
        arrowImage.enabled = false; 
        messageCanvas.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (isCooldown) return;

        playerMovement.enabled = !gameActive;

        if (gameActive && !isPlayerLocked && !win)
        {
            if (gameStarted)
            {
                HandleKeyPresses();
                if (isPressingKey)
                {
                    elapsedTime += Time.deltaTime;
                    UpdateSprites();
                }
            }
        }
        else if (!IsGameActive() && !hasStartedMiniGame && Input.GetKeyDown(KeyCode.E) && IsReadyToStart() && IsFoodAvailable())
        {
            StartMiniGame();
        }
    }

    void HandleKeyPresses()
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
                CheckElapsedTime();
            }
        }
    }

    void CheckElapsedTime()
    {
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
            Debug.LogError("Interaction Area has not been assigned!");
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

                case "BreadCarrotSticks":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("FriedCarrotBreadSticks");
                    return true;
            }
            
            StartCoroutine(ShowArrowImageForDuration(3f, $"You cannot fry a {inventoryItem.item.Name}!")); 
            audioSource.PlayOneShot(errorSound);
            return false;
        }
        else 
        {
            StartCoroutine(ShowArrowImageForDuration(3f, "Your inventory is empty!"));
            audioSource.PlayOneShot(errorSound);
            return false;
        }
    }

    IEnumerator ShowArrowImageForDuration(float duration, string message)
    {
        informationImage.enabled = true;
        arrowImage.enabled = true; 
        messageText.text = message;
        messageCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        informationImage.enabled = false;
        arrowImage.enabled = false; 
        messageCanvas.gameObject.SetActive(false);
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}
