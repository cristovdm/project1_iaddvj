using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaUrchin : MonoBehaviour
{
    public float movementSpeed = 4f;        
    public float wanderSpeed = 2f;
    public float waterDropInterval = 0.5f;
    public float playerDetectionRadius = 100f;
    public int requiredPressesToEliminate = 10;
    public GameObject waterPrefab;
    public GameObject bubblePrefab;
    public Canvas bubbleCanvas;
    public AudioClip hitSound;

    private GameObject player;
    private Rigidbody2D rb;
    private int pressCount = 0;
    private bool isNearPlayer = false;
    private Vector3 reducedScale = new Vector3(1.6f, 1.6f, 1f);
    private AudioSource audioSource;

    private Vector2 wanderDirection;
    private float wanderTimer;
    private float wanderChangeInterval = 2f;
    [SerializeField] private GameObject deathPrefab; // Prefab generado al morir
    private List<GameObject> bubbles = new List<GameObject>();

    public delegate void SeaUrchinDestroyed();
    public static event SeaUrchinDestroyed OnSeaUrchinDestroyed;

    private enum State { Wander, Flee }
    private State currentState;
    private float bubbleTimer = 10f;

    void Awake()
    {
        if (bubbleCanvas == null)
        {
            bubbleCanvas = FindObjectOfType<Canvas>();
            if (bubbleCanvas == null)
            {
                Debug.LogError("No se encontró un Canvas en la escena.");
            }
            else
            {
                bubbleCanvas.scaleFactor = 23f;
            }
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Buscar el Canvas si no está asignado
        if (bubbleCanvas == null)
        {
            bubbleCanvas = FindObjectOfType<Canvas>();
            if (bubbleCanvas == null)
            {
                Debug.LogError("No se encontró un Canvas en la escena.");
            }
        }

        currentState = State.Wander;
        wanderDirection = Random.insideUnitCircle.normalized;
        wanderTimer = wanderChangeInterval;

        StartCoroutine(DropWaterRoutine());
        StartCoroutine(StartBubbleRoutine());
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
        if (Vector2.Distance(transform.position, player.transform.position) <= playerDetectionRadius)
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
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            wanderDirection = GetRandomWanderDirection();
            wanderTimer = wanderChangeInterval;
        }

        Vector2 newPosition = rb.position + wanderDirection * wanderSpeed * Time.deltaTime;
        rb.MovePosition(newPosition);
    }

    Vector2 GetRandomWanderDirection()
    {
        Vector2 newDirection = Random.insideUnitCircle.normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, newDirection, wanderChangeInterval, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            newDirection = Vector2.Reflect(newDirection, hit.normal);
        }
        return newDirection;
    }

    void Flee()
    {
        Vector2 direction = (transform.position - player.transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, movementSpeed * Time.deltaTime, LayerMask.GetMask("Wall"));

        if (hit.collider == null)
        {
            rb.velocity = direction * movementSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        if (Input.GetKeyDown(KeyCode.Q) && isNearPlayer)
        {
            pressCount++;
            PlayHitSound();
            if (pressCount >= requiredPressesToEliminate)
            {
                GeneratePrefab();
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DropWaterRoutine()
    {
        while (true)
        {
            DropWater();
            yield return new WaitForSeconds(waterDropInterval);
        }
    }

    void DropWater()
    {
        GameObject water = Instantiate(waterPrefab, transform.position, Quaternion.identity);
        water.transform.localScale = reducedScale;
        Destroy(water, 10f);
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector2.zero;
        }
    }

    void GeneratePrefab()
    {
        if (deathPrefab != null)
        {
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnDestroy()
    {
        OnSeaUrchinDestroyed?.Invoke();
        pressCount = 0;
        StopAllCoroutines();
        ClearBubbles();
    }

    void ClearBubbles()
    {
        foreach (var bubble in bubbles)
        {
            if (bubble != null)
            {
                Destroy(bubble);
            }
        }
        bubbles.Clear();
    }

    IEnumerator StartBubbleRoutine()
    {
        yield return new WaitForSeconds(bubbleTimer);

        while (true)
        {
            int rows = 1;
            int columns = 25;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    CreateBubble(row, col);
                }
            }

            yield return new WaitForSeconds(Random.Range(0.2f, 0.7f));
        }
    }

    void CreateBubble(int row, int column)
    {
        if (bubbleCanvas == null || bubblePrefab == null) return;

        GameObject bubbleGO = Instantiate(bubblePrefab);
        bubbles.Add(bubbleGO);
        bubbleGO.transform.SetParent(bubbleCanvas.transform, false);
        RectTransform bubbleRect = bubbleGO.GetComponent<RectTransform>();
        SpriteRenderer bubbleSpriteRenderer = bubbleGO.GetComponent<SpriteRenderer>();

        // Establecer la escala del RectTransform de la burbuja
        bubbleRect.localScale = new Vector3(25f, 25f, 1f);

        float horizontalSpacing = bubbleCanvas.GetComponent<RectTransform>().rect.width / 9f;
        float verticalSpacing = bubbleCanvas.GetComponent<RectTransform>().rect.height / 18f;
        bubbleRect.anchoredPosition = new Vector2(
            -bubbleCanvas.GetComponent<RectTransform>().rect.width / 2 + horizontalSpacing * column,
            -bubbleCanvas.GetComponent<RectTransform>().rect.height / 2 - bubbleRect.rect.height / 2 + verticalSpacing * row
        );

        bubbleSpriteRenderer.color = RandomColor();

        StartCoroutine(MoveBubble(bubbleGO, bubbleRect));
    }

    Color RandomColor()
    {
        float r = Random.Range(0.2f, 0.6f);
        float g = Random.Range(0.4f, 0.8f);
        float b = Random.Range(0.6f, 1f);
        return new Color(r, g, b);
    }


    IEnumerator MoveBubble(GameObject bubbleObject, RectTransform bubbleRect)
    {
        float startTime = Time.time;
        float speed = 13f;

        float amplitude = Random.Range(10f, 25f);

        bool moveHorizontally = Random.value < 0.8f;

        while (bubbleObject != null && bubbleRect != null)
        {
            float elapsed = Time.time - startTime;
            if (bubbleRect != null)
            {
                bubbleRect.anchoredPosition += Vector2.up * speed * Time.deltaTime;
                if (moveHorizontally)
                {
                    bubbleRect.anchoredPosition += Vector2.right * Mathf.Sin(elapsed * 2f) * amplitude * Time.deltaTime;
                }
                else
                {
                    bubbleRect.anchoredPosition += Vector2.right * Mathf.Sin(elapsed * 2f) * 0.6f * Time.deltaTime;
                }
            }
            yield return null;

            if (bubbleRect != null && bubbleRect.anchoredPosition.y > bubbleCanvas.GetComponent<RectTransform>().rect.height / 2)
            {
                Destroy(bubbleObject);
                bubbles.Remove(bubbleObject);
            }
        }
    }
}
