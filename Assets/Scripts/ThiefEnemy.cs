using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Inventory.Model;

public class ThiefEnemy : MonoBehaviour
{
    public float stealInterval = 20f;
    public float horizontalFlipInterval = 1f;
    public int killKeyPressCount = 10;
    public AudioClip hitSound;
    public AudioClip destroySound; // Sonido de destrucción
    public AudioClip pickedSound;
    [SerializeField] private GameObject coinPrefab;

    public GameObject BastonesPrefab;  
    public GameObject BastonesZanahoriaApanados;
    public GameObject CarrotCakePrefab;
    public GameObject CarrotPrefab; 
    public GameObject CazuelaMarinaPrefab; 
    public GameObject CornSoupPrefab; 
    public GameObject CutCarrotPrefab; 
    public GameObject CutCornPrefab; 
    public GameObject CutTomatoPrefab; 
    public GameObject EggPrefab;
    public GameObject EnsaladaColoridaPrefab; 
    public GameObject EnsaladaConHuevoPrefab;
    public GameObject FishPrefab; 
    public GameObject TomatoPrefab; 
    public GameObject FriedEggPrefab; 
    public GameObject FriedFishPrefab; 
    public GameObject HuevoDuroPrefab; 
    public GameObject PanCortadoPrefab; 
    public GameObject PanCortadoFritoPrefab; 
    public GameObject PanPrefab;
    public GameObject PescadoHornoPrefab;
    public GameObject PescadoCalderoPrefab; 
    public GameObject SaladPrefab; 
    public GameObject SandwichPrefab; 
    public GameObject sopaTomateCrotonesPrefab; 
    public GameObject sopaZanahoriaPrefab; 
    public GameObject sopaZanahoriaHuevoPrefab; 
    public GameObject TomaticanPrefab; 
    public GameObject TomatoSoupPrefab; 
    public GameObject TortillaZanahoriaPrefab; 

    public GameObject cornPrefab; 
    public GameObject rottenTomatoPrefab;
    public GameObject rottenPanPrefab; 
    public GameObject rottenCarrotPrefab; 
    public GameObject rottenCornerPrefab;
    public GameObject rottenFishPrefab;
    public GameObject rottenEggPrefab;

    public GameObject interactionArea;
    public float speed = 10f;
    public float obstacleAvoidanceRadius = 0.5f;
    public LayerMask obstacleLayer;

    private int currentKeyPressCount = 0;
    private InventoryController inventoryController;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerNear = false;
    public AudioSource audioSource;
    private BoxCollider2D interactionCollider;
    private Vector3 initialPosition;
    private bool isReturning = false;
    private bool isBeingDestroyed = false;
    private InventoryItem itemRobbed; 

    private bool destroyedByMe = false; 

    [SerializeField]
    private InventorySO trashinventoryData;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventoryController = FindObjectOfType<InventoryController>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        interactionCollider = interactionArea.GetComponent<BoxCollider2D>();
        if (interactionCollider == null)
        {
            Debug.LogError("InteractionArea does not have a BoxCollider2D component.");
        }

        initialPosition = transform.position;

        StartCoroutine(StealItemRoutine());
        StartCoroutine(HorizontalFlipRoutine());
        StartCoroutine(MoveToTarget(interactionArea.transform.position));
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Q))
        {
            currentKeyPressCount++;
            PlayHitSound();
            if (currentKeyPressCount >= killKeyPressCount && !isBeingDestroyed)
            {
                destroyedByMe = true; 
                DestroyEnemy();
            }
        }
    }

    IEnumerator StealItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stealInterval);
            StealItem();
        }
    }

    IEnumerator HorizontalFlipRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(horizontalFlipInterval);
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    void StealItem()
    {
        if (inventoryController != null)
        {
            var trashItems = inventoryController.GetTrashInventoryData().GetCurrentInventoryState();
            if (trashItems.Count > 0)
            {
                List<int> keys = new List<int>(trashItems.Keys);
                int randomIndex = Random.Range(0, keys.Count);
                int itemKey = keys[randomIndex];
                inventoryController.GetTrashInventoryData().RemoveItem(itemKey, 1);
                itemRobbed = trashItems[itemKey]; // Asigna el item robado a la variable de clase
                itemRobbed.quantity = 1; 
                Debug.Log($"Stole {itemRobbed.item.Name} from trash inventory.");

                StartCoroutine(MoveToTarget(initialPosition, itemRobbed));
            }
            else
            {
                StartCoroutine(MoveToTarget(initialPosition));
            }
        }
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 direction = (target - transform.position).normalized;
            direction = AvoidObstacles(direction);
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
        if (isReturning)
        {
            isReturning = false;
            Destroy(gameObject);
        }
        else
        {
            isReturning = true;
            StealItem();
        }
    }

    IEnumerator MoveToTarget(Vector3 target, InventoryItem itemRobbed)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 direction = (target - transform.position).normalized;
            direction = AvoidObstacles(direction);
            transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
        if (isReturning)
        {
            isReturning = false;
            if (destroyedByMe){
                GeneratePrefab();
                destroyedByMe = false; 
            }
            Destroy(gameObject);
        }
        else
        {
            isReturning = true;
            StealItem();
        }
    }

    Vector3 AvoidObstacles(Vector3 targetDirection)
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, obstacleAvoidanceRadius, targetDirection, obstacleAvoidanceRadius, obstacleLayer);
        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;
            Vector2 avoidanceDirection = Vector2.Reflect(targetDirection, hitNormal);
            return avoidanceDirection.normalized;
        }
        return targetDirection.normalized;
    }

    void DestroyEnemy()
    {
        if (!isBeingDestroyed) // Check to ensure destruction happens only once
        {
            isBeingDestroyed = true;
            if (destroyedByMe){
                GeneratePrefab();
                destroyedByMe = false; 
            }
            Destroy(gameObject);
        }
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    void GeneratePrefab()
    {
        GameObject prefabToInstantiate = null;

        if (!itemRobbed.IsEmpty && isReturning)
        {
            switch (itemRobbed.item.Name)
            {
                case "Rotten Carrot":
                    prefabToInstantiate = rottenCarrotPrefab;
                    break;
                case "Rotten Corn":
                    prefabToInstantiate = rottenCornerPrefab;
                    break;
                case "Rotten Fish":
                    prefabToInstantiate = rottenFishPrefab;
                    break;
                case "Rotten Pan":
                    prefabToInstantiate = rottenPanPrefab;
                    break;
                case "Rotten Tomato":
                    prefabToInstantiate = rottenTomatoPrefab;
                    break;
                case "RottenEgg":
                    prefabToInstantiate = rottenEggPrefab;
                    break;
                case "Bastones":
                    prefabToInstantiate = BastonesPrefab;
                    break;
                case "BreadCarrotSticks":
                    prefabToInstantiate = BastonesZanahoriaApanados;
                    break;
                case "Carrot Cake":
                    prefabToInstantiate = CarrotCakePrefab;
                    break;
                case "Carrot":
                    prefabToInstantiate = CarrotPrefab;
                    break;
                case "Cazuela Marina":
                    prefabToInstantiate = CazuelaMarinaPrefab;
                    break;
                case "Corn Soup":
                    prefabToInstantiate = CornSoupPrefab;
                    break;
                case "Cut Carrot":
                    prefabToInstantiate = CutCarrotPrefab;
                    break;
                case "Cut Corn":
                    prefabToInstantiate = CutCornPrefab;
                    break;
                case "Cut Tomato":
                    prefabToInstantiate = CutTomatoPrefab;
                    break;
                case "Egg":
                    prefabToInstantiate = EggPrefab;
                    break;
                case "CarrotTomatoSalad":
                    prefabToInstantiate = EnsaladaColoridaPrefab;
                    break;
                case "SaladWithEgg":
                    prefabToInstantiate = EnsaladaConHuevoPrefab;
                    break;
                case "Fish":
                    prefabToInstantiate = FishPrefab;
                    break;
                case "Tomato":
                    prefabToInstantiate = TomatoPrefab;
                    break;
                case "FriedEgg":
                    prefabToInstantiate = FriedEggPrefab;
                    break;
                case "FriedFish":
                    prefabToInstantiate = FriedFishPrefab;
                    break;
                case "HuevoDuro":
                    prefabToInstantiate = HuevoDuroPrefab;
                    break;
                case "PanCortado":
                    prefabToInstantiate = PanCortadoPrefab;
                    break;
                case "PanCortadoFrito":
                    prefabToInstantiate = PanCortadoFritoPrefab;
                    break;
                case "Pan":
                    prefabToInstantiate = PanPrefab;
                    break;
                case "PescadoHorno":
                    prefabToInstantiate = PescadoHornoPrefab;
                    break;
                case "PescadoCaldero":
                    prefabToInstantiate = PescadoCalderoPrefab;
                    break;
                case "Salad":
                    prefabToInstantiate = SaladPrefab;
                    break;
                case "Sandwich":
                    prefabToInstantiate = SandwichPrefab;
                    break;
                case "SopaTomateCrotones":
                    prefabToInstantiate = sopaTomateCrotonesPrefab;
                    break;
                case "CarrotSoup":
                    prefabToInstantiate = sopaZanahoriaPrefab;
                    break;
                case "CarrotSoupEgg":
                    prefabToInstantiate = sopaZanahoriaHuevoPrefab;
                    break;
                case "Tomatican":
                    prefabToInstantiate = TomaticanPrefab;
                    break;
                case "Tomato Soup":
                    prefabToInstantiate = TomatoSoupPrefab;
                    break;
                case "tortillaZanahoria":
                    prefabToInstantiate = TortillaZanahoriaPrefab;
                    break;
                default:
                    prefabToInstantiate = coinPrefab;
                    break;
            }
        }
        else
        {
            prefabToInstantiate = coinPrefab;

        }

        if (prefabToInstantiate != null)
        {

            if (!trashinventoryData.IsInventoryFull()){
                if (!itemRobbed.IsEmpty){
                    audioSource.PlayOneShot(pickedSound);
                    GameObject instance = Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
                    MoveToTarget moveToTarget = instance.AddComponent<MoveToTarget>();
                    moveToTarget.targetPosition = interactionArea.transform.position;
                    trashinventoryData.AddItem(itemRobbed); 
                }

                else{

                    prefabToInstantiate = coinPrefab;
                    GameObject instance = Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
                }
                
            }
            else{
                prefabToInstantiate = coinPrefab;
                GameObject instance = Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            currentKeyPressCount = 0;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            currentKeyPressCount = 0;
        }
    }
}
