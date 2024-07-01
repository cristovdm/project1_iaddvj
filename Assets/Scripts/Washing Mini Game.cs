using Inventory.Model;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WashingMiniGame : MonoBehaviour
{
    public GameObject ParentObject;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI modeText;
    public GameObject upKeySprite;
    public GameObject downKeySprite;
    public GameObject leftKeySprite;
    public GameObject rightKeySprite;

    public int ScrubTotalKeySequences = 4;
    public int WashTotalKeySequences = 4;

    private int scrubTotalKeyPairs;
    private int washTotalKeyPairs;

    private bool scrubMode = false;
    private bool washMode = false;

    private int scrubNextKeyPress = 0;
    private int washNextKeyPress = 0;

    private bool gameActive = false;
    private bool isPlayerLocked = true;
    private bool readyToStart = true;
    private bool win = false;

    public AudioClip bubble;
    public AudioClip errorSound;
    private AudioSource audioSource;
    public BoxCollider2D interactionArea;
    private bool hasStartedMiniGame = false;
    private bool isCooldown = false;
    public PlayerMovement playerMovement;

    private InventoryController inventory;
    private ItemSO cleanedItem;

    [SerializeField] private Canvas messageCanvas;
    [SerializeField] private Image arrowImage;
    [SerializeField] private Image informationImage;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Canvas invisibleWall;

    void Start()
    {
        inventory = FindObjectOfType<InventoryController>();
        invisibleWall.gameObject.SetActive(false);
        SetChildrenActive(ParentObject, false);
        audioSource = GetComponent<AudioSource>();
        informationImage.enabled = false;
        arrowImage.enabled = false;
        messageCanvas.gameObject.SetActive(false);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (interactionArea == null)
        {
            interactionArea = GetComponent<BoxCollider2D>();
        }
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

        if (gameActive && !isPlayerLocked)
        {
            if (scrubMode)
            {
                scrubSequence();
            }
            else if (washMode)
            {
                washSequence();
            }
        }
        else
        {
            if (!IsGameActive() && !hasStartedMiniGame && Input.GetKeyDown(KeyCode.E) && IsReadyToStart() && IsFoodAvailable())
            {
                StartMiniGame();
            }
        }
    }

    void scrubSequence()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HandleKeyPressScrub(0, false);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HandleKeyPressScrub(1, true);
        }
    }

    void washSequence()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            HandleKeyPressWash(0, false);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HandleKeyPressWash(1, false);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            HandleKeyPressWash(2, false);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            HandleKeyPressWash(3, true);
        }
    }

    void HandleKeyPressScrub(int keyIndex, bool endSequence)
    {
        if (scrubNextKeyPress == keyIndex)
        {
            audioSource.PlayOneShot(bubble);
            scrubNextKeyPress = (scrubNextKeyPress + 1) % 2; // Ciclo 0 y 1
            ToggleKeySpriteScrub(scrubNextKeyPress);
            if (endSequence)
            {
                scrubTotalKeyPairs++;
                ToggleKeySpriteScrub(scrubNextKeyPress);
                if (scrubTotalKeyPairs >= ScrubTotalKeySequences)
                {
                    SetKeySpritesActive(false);
                    modeText.text = "WASH!";
                    instructionText.text = "Press UP, RIGHT, DOWN, LEFT in order!";
                    scrubMode = false;
                    washMode = true;
                    gameActive = true;
                    ResetKeySprites();
                    StartCoroutine(CountdownToChange());
                    upKeySprite.SetActive(true);
                }
            }
        }
    }

    void HandleKeyPressWash(int keyIndex, bool endSequence)
    {
        if (washNextKeyPress == keyIndex)
        {
            audioSource.PlayOneShot(bubble);
            washNextKeyPress = (washNextKeyPress + 1) % 4; // Ciclo 0, 1, 2, 3
            ToggleKeySpriteWash(washNextKeyPress);
            if (endSequence)
            {
                washTotalKeyPairs++;
                ToggleKeySpriteWash(washNextKeyPress);
                if (washTotalKeyPairs >= WashTotalKeySequences)
                {
                    SetKeySpritesActive(false);
                    modeText.text = "";
                    instructionText.text = "COMPLETE";
                    scrubMode = false;
                    washMode = false;
                    StartCoroutine(EndMiniGameAfterDelay());
                }
            }
        }
    }

    IEnumerator CountdownToChange()
    {
        instructionText.text = "";
        yield return new WaitForSeconds(1f);
        upKeySprite.SetActive(true);
        gameActive = true;
    }

    IEnumerator EndMiniGameAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
        EndMiniGame();
    }

    void ToggleKeySpriteScrub(int keyIndex)
    {
        SetKeySpritesActive(false);

        switch (keyIndex)
        {
            case 0:
                rightKeySprite.SetActive(true);
                break;
            case 1:
                leftKeySprite.SetActive(true);
                break;
        }
    }

    void ToggleKeySpriteWash(int keyIndex)
    {
        SetKeySpritesActive(false);

        switch (keyIndex)
        {
            case 0:
                upKeySprite.SetActive(true);
                break;
            case 1:
                rightKeySprite.SetActive(true);
                break;
            case 2:
                downKeySprite.SetActive(true);
                break;
            case 3:
                leftKeySprite.SetActive(true);
                break;
        }
    }

    void SetKeySpritesActive(bool active)
    {
        upKeySprite.SetActive(active);
        downKeySprite.SetActive(active);
        leftKeySprite.SetActive(active);
        rightKeySprite.SetActive(active);
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

    void SetChildrenActive(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

    public void StartMiniGame()
    {
        inventory.TakeOutFirstItem();
        scrubMode = true;
        washMode = false;
        hasStartedMiniGame = true;
        SetChildrenActive(ParentObject, true);
        ResetKeySprites();
        instructionText.text = "Press RIGHT, LEFT in order!";
        invisibleWall.gameObject.SetActive(true);
        modeText.text = "SCRUB!";
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(1f);
        instructionText.text = "";
        rightKeySprite.SetActive(true);
        gameActive = true;
        isPlayerLocked = false;
    }

    private void ResetGame()
    {
        hasStartedMiniGame = false;
        scrubNextKeyPress = 0;
        washNextKeyPress = 0;
        scrubMode = false;
        washMode = false;
        gameActive = false;
        isPlayerLocked = true;
        readyToStart = true;
        win = false;
        scrubTotalKeyPairs = 0;
        washTotalKeyPairs = 0;
    }

    void EndMiniGame()
    {
        ResetGame();
        ResetKeySprites();
        SetChildrenActive(ParentObject, false);
        invisibleWall.gameObject.SetActive(false);
        StartCooldown();
        playerMovement.enabled = true;
        InventoryItem item = new InventoryItem
        {
            item = cleanedItem,
            quantity = 1,
            itemState = new List<ItemParameter>()
        };
        inventory.AddInventoryItem(item);
    }

    void ResetKeySprites()
    {
        upKeySprite.SetActive(false);
        downKeySprite.SetActive(false);
        leftKeySprite.SetActive(false);
        rightKeySprite.SetActive(false);
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
            switch (inventoryItem.item.Name)
            {
                case "Rotten Carrot":
                    cleanedItem = ResourceManager.LoadResource<EdibleItemSO>("Carrot");
                    return true;

                case "Rotten Corn":
                    cleanedItem = ResourceManager.LoadResource<EdibleItemSO>("Corn");
                    return true;

                case "Rotten Fish":
                    cleanedItem = ResourceManager.LoadResource<EdibleItemSO>("Fish");
                    return true;

                case "Rotten Pan":
                    cleanedItem = ResourceManager.LoadResource<EdibleItemSO>("Bread");
                    return true;

                case "Rotten Tomato":
                    cleanedItem = ResourceManager.LoadResource<EdibleItemSO>("Tomato");
                    return true;

                case "RottenEgg":
                    cleanedItem = ResourceManager.LoadResource<EdibleItemSO>("Egg");
                    return true;

                default:
                    StartCoroutine(ShowArrowImageForDuration(3f, $"You cannot wash {inventoryItem.item.Name}!"));
                    audioSource.PlayOneShot(errorSound);
                    return false;
            }
        }
        else
        {
            StartCoroutine(ShowArrowImageForDuration(3f, $"Your inventory is empty!"));
            audioSource.PlayOneShot(errorSound);
            return false;
        }
    }

    IEnumerator ShowArrowImageForDuration(float duration, string message)
    {
        arrowImage.enabled = true;
        informationImage.enabled = true;
        messageText.text = message;
        messageCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        arrowImage.enabled = false;
        informationImage.enabled = false;
        messageCanvas.gameObject.SetActive(false);
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}
