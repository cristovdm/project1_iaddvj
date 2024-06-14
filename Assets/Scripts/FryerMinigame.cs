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
    public TextMeshProUGUI feedbackText;  // Referencia al objeto de texto UI de TextMeshPro
    public GameObject targetSprite;       // Referencia al objeto del sprite principal
    public GameObject additionalSprite;   // Referencia al objeto del sprite adicional
    public AudioClip activationClip;      // Clip de audio a reproducir al activar el sprite
    private AudioSource audioSource;       // AudioSource para reproducir el sonido

    public PlayerMovement playerMovement;
    private InventoryController inventory;
    private ItemSO cutItem;

    private float targetTime;
    private float elapsedTime;
    private bool isPressingKey;
    private bool gameStarted;
    private bool spriteActive;
    private bool additionalSpriteActive;   // Indica si el sprite adicional est� activo
    private const float marginOfError = 0.5f;  // Margen de error de 0.5 segundos

    

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
        audioSource = gameObject.AddComponent<AudioSource>(); // A�adir AudioSource al objeto actual

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
                    isPressingKey = true;
                    elapsedTime = 0f;
                    feedbackText.gameObject.SetActive(false); // Desactivar el texto al presionar la tecla
                    DeactivateAdditionalSprite(); // Desactivar el sprite adicional al presionar la tecla
                }

                if (Input.GetKeyUp(KeyCode.DownArrow))
                {
                    if (isPressingKey)
                    {
                        isPressingKey = false;

                        if (elapsedTime >= targetTime && elapsedTime <= targetTime + marginOfError)
                        {
                            DeactivateSprite();
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
                }

                if (elapsedTime >= targetTime && elapsedTime <= targetTime + marginOfError)
                {
                    if (!spriteActive)
                    {
                        ActivateSprite();
                    }
                }
                else
                {
                    if (spriteActive)
                    {
                        DeactivateSprite();
                    }
                }
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
        spriteActive = false;
        additionalSpriteActive = true; // Inicialmente el sprite adicional est� activo

        feedbackText.gameObject.SetActive(true); // Activar el texto al reiniciar el juego
        feedbackText.text = "Maintain the button until it's ready.";
        DeactivateSprite();
        ActivateAdditionalSprite(); // Activar el sprite adicional al reiniciar el juego
    }

    void ActivateSprite()
    {
        targetSprite.SetActive(true);
        audioSource.PlayOneShot(activationClip); // Reproducir el clip de audio una vez
        spriteActive = true;
    }

    void DeactivateSprite()
    {
        targetSprite.SetActive(false);
        spriteActive = false;
    }

    void ActivateAdditionalSprite()
    {
        additionalSprite.SetActive(true);
        additionalSpriteActive = true;
    }

    void DeactivateAdditionalSprite()
    {
        additionalSprite.SetActive(false);
        additionalSpriteActive = false;
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
        DeactivateSprite();
        ResetKeySprites();
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(1f);
        gameActive = true;
        isPlayerLocked = false;
        StartGame();
    }

    private void ResetGame()
    {
        DeactivateSprite();
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
        ResetGame();
        ResetKeySprites();
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

    void ResetKeySprites()
    {

        additionalSprite.SetActive(false);
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

                case "FriedFish":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("FriedFish");
                    return true;

                case "Fish":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("PanCortadoFrito");
                    return true;

                case "CutCorn":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("PopCorn");
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
