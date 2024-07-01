using UnityEngine;
using TMPro;
using System.Collections;
using Inventory;
using Inventory.Model;
using System.Collections.Generic;
using UnityEngine.UI; 

public class CuttingBoardMiniGame : MonoBehaviour
{
    public GameObject ParentObject;
    public TextMeshProUGUI instructionText;
    public GameObject wKeySprite;
    public GameObject sKeySprite;
    public int totalKeyPresses = 10;
    private int totalKeyPairs;
    private int keyPresses = 0;
    private int nextKeyPress = 0;  
    private bool gameActive = false;
    private bool isPlayerLocked = true;
    private bool readyToStart = true;
    private bool win = false;
    public AudioClip chop;
    public AudioClip errorSound;
    private AudioSource audioSource;
    public BoxCollider2D interactionArea;
    private bool hasStartedMiniGame = false;
    private bool isCooldown = false;
    public PlayerMovement playerMovement;
    private InventoryController inventory;
    private ItemSO cutItem;

    [SerializeField] private Canvas messageCanvas;
    [SerializeField] private Image arrowImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image informationImage;

    [SerializeField] private Canvas invisibleWall;

    void Start()
    {
        inventory = FindObjectOfType<InventoryController>();
        SetChildrenActive(ParentObject, false);
        audioSource = GetComponent<AudioSource>();
        arrowImage.enabled = false; 
        informationImage.enabled = false; 

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

        if (gameActive && !isPlayerLocked && !win)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Pressed W");
                if (nextKeyPress == 0)
                {
                    audioSource.PlayOneShot(chop);
                    ToggleKeySprite();
                    nextKeyPress = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Pressed S");
                if (nextKeyPress == 1)
                {
                    audioSource.PlayOneShot(chop);
                    keyPresses++;
                    ToggleKeySprite();
                    nextKeyPress = 0;
                }
            }

            if (keyPresses >= totalKeyPairs)
            {
                win = true;
                instructionText.text = "COMPLETE";
                wKeySprite.SetActive(false);
                sKeySprite.SetActive(false);
                Invoke("EndMiniGame", 1.0f);
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

    void ToggleKeySprite()
    {
        wKeySprite.SetActive(!wKeySprite.activeSelf);
        sKeySprite.SetActive(!sKeySprite.activeSelf);
    }

    public void StartMiniGame()
    {
        playerMovement.enabled = false;
        inventory.TakeOutFirstItem();
        hasStartedMiniGame = true;
        totalKeyPairs = totalKeyPresses;
        SetChildrenActive(ParentObject, true);
        invisibleWall.gameObject.SetActive(true);
        wKeySprite.SetActive(false);
        sKeySprite.SetActive(false);
        instructionText.text = "Press W and S in order!";
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(1f);
        instructionText.text = "";
        wKeySprite.SetActive(true);
        gameActive = true;
        isPlayerLocked = false;
    }

    private void ResetGame()
    {
        hasStartedMiniGame = false;
        totalKeyPairs = totalKeyPresses;
        keyPresses = 0;
        nextKeyPress = 0; 
        gameActive = false;
        isPlayerLocked = true;
        readyToStart = false;
        win = false;
    }

    void EndMiniGame()
    {
        ResetGame();
        wKeySprite.SetActive(false);
        sKeySprite.SetActive(false);
        SetChildrenActive(ParentObject, false);
        invisibleWall.gameObject.SetActive(false);
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
                case "Carrot":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("CutCarrot");
                    return true;

                case "Corn":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("CutCorn");
                    return true;

                case "Tomato":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("CutTomato");
                    return true;

                case "Bread":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("PanCortado");
                    return true;
            }
            StartCoroutine(ShowArrowImageForDuration(3f, $"You cannot cut a {inventoryItem.item.Name}!")); 
            audioSource.PlayOneShot(errorSound);
            return false;
        }
        else{
            StartCoroutine(ShowArrowImageForDuration(3f, "Your inventory is empty!"));
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
