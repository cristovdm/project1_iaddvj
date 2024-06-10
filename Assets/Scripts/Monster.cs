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
    [SerializeField] private CircleCollider2D triggerCollider; // Referencia al collider
    [SerializeField] private float destructionRange = 10.0f; // Ajusta seg�n sea necesario

    private AudioSource audioSource;
    private GameObject canvasChangeScene;


    void OnEnable()
    {
        Mine.OnMineExploded += HandleMineExploded;
        canvasChangeScene = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == "CanvasChangeScene");
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

        // Aseg�rate de que el collider est� configurado como trigger
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
            if (canvasChangeScene != null)
            {
                canvasChangeScene.SetActive(true);
            }
            else
            {
                Debug.LogError("Canvas Change Scene not found");
            }
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

    // Mostrar el rango de destrucci�n en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, destructionRange);
    }
}
