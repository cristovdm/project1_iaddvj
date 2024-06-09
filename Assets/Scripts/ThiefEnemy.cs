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

    private int currentKeyPressCount = 0;
    private InventoryController inventoryController;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerNear = false;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        inventoryController = FindObjectOfType<InventoryController>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        StartCoroutine(StealItemRoutine());
        StartCoroutine(HorizontalFlipRoutine());
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.K))
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
            }
        }
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
