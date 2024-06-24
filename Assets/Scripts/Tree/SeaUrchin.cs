using System.Collections;
using UnityEngine;

public class SeaUrchin : MonoBehaviour
{
    public float movementSpeed = 4f;          // Velocidad de movimiento general
    public float wanderSpeed = 2f;            // Velocidad de movimiento al vagar
    public float waterDropInterval = 0.5f;
    public float playerDetectionRadius = 100f;
    public int requiredPressesToEliminate = 10;
    public GameObject waterPrefab;
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

    public delegate void SeaUrchinDestroyed();
    public static event SeaUrchinDestroyed OnSeaUrchinDestroyed;

    private enum State { Wander, Flee }
    private State currentState;

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

        currentState = State.Wander;
        wanderDirection = Random.insideUnitCircle.normalized;
        wanderTimer = wanderChangeInterval;

        StartCoroutine(DropWaterRoutine());
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
        // Verificar que no haya una pared en la nueva dirección
        RaycastHit2D hit = Physics2D.Raycast(transform.position, newDirection, wanderChangeInterval, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            // Si hay una pared, intentar otra dirección
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

    void OnDestroy()
    {
        OnSeaUrchinDestroyed?.Invoke();
        pressCount = 0;
    }
}
