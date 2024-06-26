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
    public float obstacleAvoidanceRadius = 0.5f;
    public float moveSpeed = 5f;
    public float fleeSpeed = 8f;

    public Rigidbody2D rb;

    public float minX = -20f, minY = -20f, maxX = 20f, maxY = 20f;
    public int maxBananas = 6;

    private GameObject player;
    private int pressCount = 0;
    private bool isNearPlayer = false;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private List<GameObject> bananas = new List<GameObject>();

    public delegate void BananaOtterDestroyed();
    public static event BananaOtterDestroyed OnBananaOtterDestroyed;

    private enum State { Wander, Flee }
    private State currentState;

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

        currentState = State.Wander;

        StartCoroutine(BananaThrowAndMoveRoutine());
        StartCoroutine(FlipRoutine());
    }

    void Update()
    {
        if (player != null)
        {
            CheckPlayerProximity();
            UpdateState();
        }
    }

    void UpdateState()
    {
        switch (currentState)
        {
            case State.Wander:
                Wander();
                break;
            case State.Flee:
                Flee();
                break;
        }
    }

    void CheckPlayerProximity()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= detectionRadius)
        {
            isNearPlayer = true;
            currentState = State.Flee;
        }
        else
        {
            isNearPlayer = false;
            currentState = State.Wander;
        }
    }

    void Wander()
    {
        // Implementación del comportamiento de Wander
    }

    void Flee()
    {
        /*
        Vector2 direction = (transform.position - player.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveSpeed * Time.deltaTime, LayerMask.GetMask("Wall"));

        if (hit.collider == null)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            Debug.Log("corro");
            rb.velocity = Vector2.zero;
        }
        */

        if (Input.GetKeyDown(KeyCode.Q) && isNearPlayer)
        {
            pressCount++;
            PlayHitSound();
            if (pressCount >= requiredPressesToEliminate)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator BananaThrowAndMoveRoutine()
    {
        Vector2 position1 = new Vector2(30f, 20f);
        Vector2 position2 = new Vector2(-30f, -20f);
        Vector2 currentPosition = position1;

        while (true)
        {

            yield return StartCoroutine(MoveToTarget(currentPosition));

            for (int i = 0; i < 3; i++)
            {
                DropBanana();
                yield return new WaitForSeconds(bananaDropInterval);
            }

            currentPosition = (currentPosition == position1) ? position2 : position1;
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
        if (!IsPositionOccupied(spawnPosition, "Wall") && !IsPositionOccupied(spawnPosition, "Decoration"))
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
        } while (IsPositionOccupied(spawnPosition, "Wall") && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Could not find a valid spawn position for the banana.");
        }

        return spawnPosition;
    }

    IEnumerator MoveToTarget(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            Vector2 direction = (target - (Vector2)transform.position).normalized;
            direction = AvoidObstacles(direction);

            Vector2 futurePosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;
            if (!IsPositionOccupied(futurePosition, "Wall"))
            {
                transform.position = futurePosition;
            }
            else
            {
                // Si hay un obstáculo, intenta moverte en una dirección diferente
                direction = FindAlternativeDirection(direction);
                futurePosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;
                if (!IsPositionOccupied(futurePosition, "Wall"))
                {
                    transform.position = futurePosition;
                }
            }

            yield return null;
        }
    }

    Vector2 FindAlternativeDirection(Vector2 originalDirection)
    {
        // Intenta moverte en una dirección perpendicular a la original
        Vector2 newDirection = new Vector2(-originalDirection.y, originalDirection.x);

        // Si esta dirección está bloqueada, intenta la dirección opuesta
        Vector2 futurePosition = (Vector2)transform.position + newDirection * moveSpeed * Time.deltaTime;
        if (IsPositionOccupied(futurePosition, "Wall"))
        {
            newDirection = new Vector2(originalDirection.y, -originalDirection.x);
        }

        return newDirection.normalized;
    }

    Vector2 AvoidObstacles(Vector2 targetDirection)
    {
        float avoidanceRadius = obstacleAvoidanceRadius;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, avoidanceRadius, targetDirection, avoidanceRadius);

        if (hit.collider != null && hit.collider.CompareTag("Wall"))
        {
            Vector2 hitNormal = hit.normal;
            Vector2 avoidanceDirection = Vector2.Reflect(targetDirection, hitNormal);
            return (avoidanceDirection + targetDirection).normalized;
        }
        return targetDirection.normalized;
    }

    bool IsPositionOccupied(Vector2 position, string layerName)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, obstacleAvoidanceRadius);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer(layerName))
            {
                return true;
            }
        }
        return false;
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
