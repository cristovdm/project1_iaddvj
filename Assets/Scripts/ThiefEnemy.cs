using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

public class ThiefEnemy : MonoBehaviour
{
    public float stealInterval = 20f;
    public float horizontalFlipInterval = 1f;
    public int killKeyPressCount = 10;
    public AudioClip hitSound;

    public GameObject interactionArea; 
    public float speed = 10f;
    public float obstacleAvoidanceRadius = 0.5f;
    public LayerMask obstacleLayer;

    private int currentKeyPressCount = 0;
    private InventoryController inventoryController;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerNear = false;
    private AudioSource audioSource;
    private BoxCollider2D interactionCollider;
    private Vector3 initialPosition;
    private bool isReturning = false;

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
            if (currentKeyPressCount >= killKeyPressCount)
            {
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
                Debug.Log($"Stole {trashItems[itemKey].item.Name} from trash inventory.");
                StartCoroutine(MoveToTarget(initialPosition));
            }
            else{
                // caso donde no encuentra nada, mostrar una burbuja de enojo o algo así. 
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
        Destroy(gameObject);
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
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
