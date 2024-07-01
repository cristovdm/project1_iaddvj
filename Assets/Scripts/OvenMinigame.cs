using Inventory.Model;
using Inventory;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI; 

public class OvenMinigame : MonoBehaviour
{
    public GameObject ParentObject;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI targetValueText;
    public TextMeshProUGUI playerValueText;
    public GameObject leftKeySprite;
    public GameObject rightKeySprite;
    public BoxCollider2D interactionArea;
    public AudioClip bubble;
    public Transform fireSprite;
    private AudioSource audioSource; 

    public PlayerMovement playerMovement;
    private InventoryController inventory;
    private ItemSO cutItem;

    public float speed = 1.0f;
    private int targetValue;
    private float playerValue;
    private int iteration;
    private int totalIterations = 3;
    private float timeInRange;
    private float requiredTimeInRange = 0.5f;
    private float maxScale = 500f;

    private bool isCooldown = false;
    private bool gameActive = false;
    private bool isPlayerLocked = true;
    private bool win = false;
    private bool readyToStart = true;
    private bool hasStartedMiniGame = false;

    public AudioClip errorSound;

    [SerializeField] private Canvas messageCanvas;
    [SerializeField] private Image arrowImage;
    [SerializeField] private Image informationImage;
    [SerializeField] private TextMeshProUGUI messageText;

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

        playerValue = 0;
        iteration = 0;
        timeInRange = 0;

        SetNewTargetValue();
    }

    void Update()
    {
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
            playerValueText.text = $"{(int)playerValue}�";
            targetValueText.text = $"{targetValue}�";

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                playerValue -= speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                playerValue += speed * Time.deltaTime;
            }

            playerValue = Mathf.Clamp(playerValue, 0, 220);

            if (Mathf.Abs(playerValue - targetValue) <= 15)
            {
                playerValueText.color = Color.green;
                timeInRange += Time.deltaTime;

                if (timeInRange >= requiredTimeInRange)
                {
                    iteration++;

                    if (iteration >= totalIterations)
                    {
                        win = true;
                        fireSprite.localScale = new Vector3(0.0f, 0.0f, 1.0f);
                        instructionText.text = "COMPLETE";
                        Invoke("EndMiniGame", 1.0f);
                    }
                    else
                    {
                        playerValue = 0;
                        timeInRange = 0;
                        SetNewTargetValue();
                    }
                }
            }
            else
            {
                timeInRange = 0;
                playerValueText.color = Color.white;
            }
        }
        else
        {
            if (!IsGameActive() && !hasStartedMiniGame && Input.GetKeyDown(KeyCode.E) && IsReadyToStart() && IsFoodAvailable())
            {
                StartMiniGame();
            }
        }
        UpdateFireSpriteSize();
    }

    void UpdateFireSpriteSize()
    {
        float sizeFactor = Mathf.Clamp01(1 - Mathf.Abs(playerValue - targetValue) / 220f);
        float scale = sizeFactor * (500 - 100) + 100;
        fireSprite.localScale = new Vector3(scale, scale, 1.0f);
    }

    void SetNewTargetValue()
    {
        targetValue = Random.Range(31, 221);
    }


    void SetChildrenActive(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
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
                case "Fish":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("PescadoHorno");
                    return true;

                case "CarrotSoupEgg":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("CarrotCake");
                    return true;

                case "CutCorn":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("PopCorn");
                    return true;
            }

            StartCoroutine(ShowArrowImageForDuration(3f, $"You cannot cook with a {inventoryItem.item.Name}!")); 
            audioSource.PlayOneShot(errorSound);
            return false;
        }
        else{
            StartCoroutine(ShowArrowImageForDuration(3f, $"Your inventory is empty!")); 
            audioSource.PlayOneShot(errorSound);
            return false;
        } 
    }

    public bool IsGameActive()
    {
        return gameActive;
    }

    void ResetKeySprites()
    {
        leftKeySprite.SetActive(false);
        rightKeySprite.SetActive(false);
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(1f);
        instructionText.text = "";
        leftKeySprite.SetActive(true);
        rightKeySprite.SetActive(true);
        gameActive = true;
        isPlayerLocked = false;
    }

    public void StartMiniGame()
    {
        inventory.TakeOutFirstItem();
        hasStartedMiniGame = true;
        SetChildrenActive(ParentObject, true);
        invisibleWall.gameObject.SetActive(true);
        leftKeySprite.SetActive(true);
        rightKeySprite.SetActive(true);
        targetValueText.text = "";
        playerValueText.text = "";
        instructionText.text = "Select the Correct Temperature!";
        SetNewTargetValue();
        StartCoroutine(CountdownToStart());
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

    private void ResetGame()
    {
        hasStartedMiniGame = false;
        gameActive = false;
        isPlayerLocked = true;
        readyToStart = false;
        win = false;
        playerValue = 0;
        targetValue = 0;
        timeInRange = 0;
        iteration = 0;
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
            item = cutItem,
            quantity = 1,
            itemState = new List<ItemParameter>()
        };
        inventory.AddInventoryItem(item);

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
}
