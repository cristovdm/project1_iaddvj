using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaOtter : MonoBehaviour
{
    public GameObject bananaPrefab;
    public float bananaDropInterval = 1f;
    public int requiredPressesToEliminate = 10;
    public AudioClip hitSound;
    public float flipInterval = 1f;
    public float detectionRadius = 9f;
    public float minPlayerDistance = 8f;

    public float minX, minY, maxX, maxY;
    public int maxBananas = 6;

    private GameObject player;
    private int pressCount = 0;
    private bool isNearPlayer = false;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private List<GameObject> bananas = new List<GameObject>();

    public delegate void BananaOtterDestroyed();
    public static event BananaOtterDestroyed OnBananaOtterDestroyed;

    void Start()
    {
        maxBananas += 6;
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        StartCoroutine(DropBananaRoutine());
        StartCoroutine(FlipRoutine());
    }

    IEnumerator DropBananaRoutine()
    {
        while (true)
        {
            if (bananas.Count < maxBananas)
            {
                DropBanana();
            }
            yield return new WaitForSeconds(bananaDropInterval);
        }
    }

    IEnumerator FlipRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(flipInterval);
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    void DropBanana()
    {
        Vector2 spawnPosition = GetRandomPosition();
        if (!IsPositionOccupied(spawnPosition))
        {
            GameObject banana = Instantiate(bananaPrefab, spawnPosition, Quaternion.identity);
            bananas.Add(banana);
            StartCoroutine(RemoveBananaFromListAfterTime(banana, 20f));
        }
    }

    IEnumerator RemoveBananaFromListAfterTime(GameObject banana, float delay)
    {
        yield return new WaitForSeconds(delay);
        bananas.Remove(banana);
        Destroy(banana);
    }

    Vector2 GetRandomPosition()
    {
        Vector2 spawnPosition;
        int maxAttempts = 100;
        int attempts = 0;

        do
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            spawnPosition = new Vector2(x, y);
            attempts++;
        } while (IsPositionOccupied(spawnPosition) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find a valid spawn position for the banana.");
        }

        return spawnPosition;
    }

    bool IsPositionOccupied(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Wall") || collider.CompareTag("Food") || collider.CompareTag("Player"))
            {
                return true;
            }
        }
        if (Vector2.Distance(position, player.transform.position) < minPlayerDistance)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        if (player != null)
        {
            CheckPlayerProximity();
        }
    }

    void CheckPlayerProximity()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= detectionRadius)
        {
            isNearPlayer = true;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                pressCount++;
                PlayHitSound();
                if (pressCount >= requiredPressesToEliminate)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            isNearPlayer = false;
            pressCount = 0;
        }
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    void OnDestroy()
    {
        OnBananaOtterDestroyed?.Invoke();
        pressCount = 0;
    }
}
