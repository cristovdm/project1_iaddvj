using UnityEngine;
using UnityEngine.SceneManagement;  // Necesario para cambiar escenas

public class EnemyAI2D : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private CircleCollider2D triggerCollider;  // Referencia al collider
    private AudioSource audioSource;

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
}
