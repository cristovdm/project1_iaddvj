using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private CircleCollider2D triggerCollider; // Referencia al collider
    [SerializeField] private float destructionRange = 10.0f; // Ajusta según sea necesario

    private AudioSource audioSource;

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
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        audioSource = GetComponent<AudioSource>();

        // Asegúrate de que el collider está configurado como trigger
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<CircleCollider2D>();
        }

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
            Debug.Log("Trigger collider radius: " + triggerCollider.radius);
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
            SceneManager.LoadScene("Kitchen");
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

    // Mostrar el rango de destrucción en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, destructionRange);
    }
}
