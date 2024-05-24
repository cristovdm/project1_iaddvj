using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Inventory;
using Inventory.Model;

public class WashingMiniGame : MonoBehaviour
{
    public GameObject ParentObject;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI modeText;
    public GameObject upKeySprite;
    public GameObject downKeySprite;
    public GameObject leftKeySprite;
    public GameObject rightKeySprite;

    public int ScrubtotalKeySequences = 4; //Cantidad total de sequencias a completar Scrub
    public int WashtotalKeySequences = 4; //Cantidad total de sequencias a completar Wash
    private int ScrubtotalKeyPairs; //Variable que cumple el mismo objetivo que "totalKeySequences", para evitar modificar este mencionado.
    private int WashtotalKeyPairs;

    private bool scrub = false;
    private bool wash = false;

    private int scrubNextKeyPress = 0; // 0 => RIGHT; 1 => LEFT;
    private int washNextKeyPress = 0; // 0 => UP; 1 => RIGHT; 2 => DOWN; 3 => LEFT
    
    private bool gameActive = false;
    private bool isPlayerLocked = true;
    private bool readyToStart = true;
    private bool win = false;
    public AudioClip bubble;
    private AudioSource audioSource;
    public BoxCollider2D interactionArea;
    private bool hasStartedMiniGame = false;
    private bool isCooldown = false;
    public PlayerMovement playerMovement;

    private InventoryController inventory;
    private ItemSO cleanedItem;

    void Start()
    {
        inventory = FindObjectOfType<InventoryController>();
        SetChildrenActive(ParentObject, false);
        audioSource = GetComponent<AudioSource>();
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

        // Disable player movement when the mini-game is active
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
            if (scrub)
            {
                scrubSequence();
            }
            else if (wash) {
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
            scrubNextKeyPress = (scrubNextKeyPress + 1);
            ToggleKeySpriteScrub(scrubNextKeyPress);
            if (endSequence)
            {
                scrubNextKeyPress = 0;
                ScrubtotalKeyPairs++;
                ToggleKeySpriteScrub(scrubNextKeyPress);
                if (ScrubtotalKeyPairs >= ScrubtotalKeySequences)
                {
                    upKeySprite.SetActive(false);
                    downKeySprite.SetActive(false);
                    leftKeySprite.SetActive(false);
                    rightKeySprite.SetActive(false);
                    modeText.text = "WASH!";
                    instructionText.text = "Press UP, RIGHT, DOWN, LEFT in order!";
                    scrub = false;
                    wash = true;
                    gameActive = true;
                    ResetKeySprites();
                    StartCoroutine(CountdownToChange());
                    upKeySprite.SetActive(true);
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



void HandleKeyPressWash(int keyIndex, bool endSequence)
    {
        if (washNextKeyPress == keyIndex)
        {
            audioSource.PlayOneShot(bubble);
            washNextKeyPress = (washNextKeyPress + 1);
            ToggleKeySpriteWash(washNextKeyPress);
            if (endSequence)
            {
                washNextKeyPress = 0;
                WashtotalKeyPairs++;
                ToggleKeySpriteWash(washNextKeyPress);
                if (WashtotalKeyPairs >= WashtotalKeySequences)
                {
                    upKeySprite.SetActive(false);
                    downKeySprite.SetActive(false);
                    leftKeySprite.SetActive(false);
                    rightKeySprite.SetActive(false);
                    modeText.text = "";
                    instructionText.text = "COMPLETE";
                    scrub = false;
                    wash = false;
                    StartCoroutine(EndMiniGameAfterDelay());
                }
            }
        }
    }

    IEnumerator EndMiniGameAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
        EndMiniGame();
    }

    void ToggleKeySpriteScrub(int keyIndex)
    {
        // Reset all key sprites
        upKeySprite.SetActive(false);
        downKeySprite.SetActive(false);
        leftKeySprite.SetActive(false);
        rightKeySprite.SetActive(false);

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
        // Reset all key sprites
        upKeySprite.SetActive(false);
        downKeySprite.SetActive(false);
        leftKeySprite.SetActive(false);
        rightKeySprite.SetActive(false);

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
        scrub = true;
        wash = false;
        hasStartedMiniGame = true;
        SetChildrenActive(ParentObject, true);
        ResetKeySprites();
        instructionText.text = "Press RIGHT, LEFT in order!";
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
        gameActive = false;
        isPlayerLocked = true;
        readyToStart = true;
        win = false;
        scrub = false;
        wash = false;
        ScrubtotalKeyPairs = 0;
        WashtotalKeyPairs = 0;
    }

    void EndMiniGame()
    {
        ResetGame();
        ResetKeySprites();
        SetChildrenActive(ParentObject, false);
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
        //Revisar si hay comida lavable
        InventoryItem inventoryItem = inventory.GetInventoryFirstItem();

        if (!inventoryItem.IsEmpty)
        {
            Debug.Log(inventoryItem.item.Name);
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
            }
            return false;
        }
        else return false;
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}
