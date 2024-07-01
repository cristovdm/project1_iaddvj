using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class EnemyAI2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private CircleCollider2D triggerCollider;
    [SerializeField] private float destructionRange = 10.0f;

    private AudioSource audioSource;

    [SerializeField]
    private CountdownTimer countdownTimer;


    void OnEnable()
    {
        Mine.OnMineExploded += HandleMineExploded;
    }

    void OnDisable()
    {
        Mine.OnMineExploded -= HandleMineExploded;
    }

    void Start()
    {
        countdownTimer = FindObjectOfType<CountdownTimer>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        audioSource = GetComponent<AudioSource>();

        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<CircleCollider2D>();
        }

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    void Update()
    {
        MoveTowardsPlayer();
        CheckPlayerProximity();
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveSpeed * Time.deltaTime, wallLayer);

            if (hit.collider == null)
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
    }

    void CheckPlayerProximity()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            PlayAlertSound();
        }
    }

    void PlayAlertSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(alertSound);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            countdownTimer.SetCountdownToZero();
        }
    }

    void HandleMineExploded(Vector2 minePosition)
    {
        Debug.Log("EXPLODED");
        float distanceToMine = Vector2.Distance(transform.position, minePosition);
        Debug.Log("Distance to mine: " + distanceToMine);

        if (distanceToMine <= destructionRange)
        {
            Debug.Log("Enemy within destruction range, destroying.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Enemy out of destruction range.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, destructionRange);
    }
}
