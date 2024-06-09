using System.Collections;
using UnityEngine;

public class SeaUrchin : MonoBehaviour
{
    public float movementSpeed = 4f;
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

    public delegate void SeaUrchinDestroyed();
    public static event SeaUrchinDestroyed OnSeaUrchinDestroyed;

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

        StartCoroutine(DropWaterRoutine());
    }

    void Update()
    {
        if (player != null)
        {
            MoveAwayFromPlayer();
            CheckPlayerProximity();
        }
    }

    void MoveAwayFromPlayer()
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
    }

    void CheckPlayerProximity()
    {
        if (Vector2.Distance(transform.position, player.transform.position) <= playerDetectionRadius)
        {
            isNearPlayer = true;
            if (Input.GetKeyDown(KeyCode.K))
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
