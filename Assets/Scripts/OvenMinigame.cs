using Inventory.Model;
using Inventory;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;

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

    public float speed = 1.0f;  // Velocidad a la que cambia el valor del jugador.
    private int targetValue;    // Valor objetivo seleccionado aleatoriamente.
    private float playerValue;  // Valor actual del jugador.
    private int iteration;      // Contador de iteraciones completadas.
    private int totalIterations = 3;  // Número total de iteraciones necesarias para ganar.
    private float timeInRange;  // Tiempo que el jugador ha permanecido dentro del rango.
    private float requiredTimeInRange = 0.5f;  // Tiempo necesario dentro del rango para completar una iteración.
    private float maxScale = 500f;

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
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (interactionArea == null)
        {
            interactionArea = GetComponent<BoxCollider2D>();
        }


        // Inicializar el valor del jugador a 0 y el contador de iteraciones.
        playerValue = 0;
        iteration = 0;
        timeInRange = 0;

        // Seleccionar el primer valor objetivo aleatorio.
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
            playerValueText.text = $"{(int)playerValue}°";
            targetValueText.text = $"{targetValue}°";
            // Obtener la entrada del jugador.
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                // Reducir el valor del jugador.
                playerValue -= speed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                // Aumentar el valor del jugador.
                playerValue += speed * Time.deltaTime;
            }

            // Limitar el valor del jugador entre 0 y 220.
            playerValue = Mathf.Clamp(playerValue, 0, 220);

            // Verificar si el valor del jugador está dentro del rango de -15 y +15 del valor objetivo.
            if (Mathf.Abs(playerValue - targetValue) <= 15)
            {
                playerValueText.color = Color.green;
                // Incrementar el temporizador de tiempo en rango.
                timeInRange += Time.deltaTime;

                // Verificar si el jugador ha estado dentro del rango el tiempo suficiente.
                if (timeInRange >= requiredTimeInRange)
                {
                    // Incrementar el contador de iteraciones.
                    iteration++;

                    // Verificar si se han completado todas las iteraciones.
                    if (iteration >= totalIterations)
                    {
                        win = true;
                        fireSprite.localScale = new Vector3(0.0f, 0.0f, 1.0f);
                        instructionText.text = "COMPLETE";
                        Invoke("EndMiniGame", 1.0f);
                    }
                    else
                    {
                        Debug.Log("¡Completaste una iteración! Iteraciones actuales: " + iteration);
                        // Reiniciar el valor del jugador a 0, el temporizador y seleccionar un nuevo valor objetivo.
                        playerValue = 0;
                        timeInRange = 0;
                        SetNewTargetValue();
                    }
                }
            }
            else
            {
                // Reiniciar el temporizador si el jugador sale del rango.
                timeInRange = 0;
                playerValueText.color = Color.white;
            }
        }
        else
        {
            Debug.Log(IsReadyToStart());
            if (!IsGameActive() && !hasStartedMiniGame && Input.GetKeyDown(KeyCode.E) && IsReadyToStart())
            {
                StartMiniGame();
            }
        }
        UpdateFireSpriteSize();
    }

    void UpdateFireSpriteSize()
    {
        // Calcular el tamaño del sprite de fuego en función de la diferencia entre el valor del jugador y el valor objetivo.
        float sizeFactor = Mathf.Clamp01(1 - Mathf.Abs(playerValue - targetValue) / 220f); // Normalizar la diferencia al rango [0, 1]
        float scale = sizeFactor * (500 - 100) + 100;
        fireSprite.localScale = new Vector3(scale, scale, 1.0f);
    }

    void SetNewTargetValue()
    {
        // Seleccionar un nuevo valor objetivo entre 31 y 220.
        targetValue = Random.Range(31, 221);
        Debug.Log("Nuevo valor objetivo: " + targetValue);
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
                case "Cut Tomato":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("TomatoSoup");
                    return true;

                case "Corn":
                    cutItem = ResourceManager.LoadResource<EdibleItemSO>("CornSoup");
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
}
