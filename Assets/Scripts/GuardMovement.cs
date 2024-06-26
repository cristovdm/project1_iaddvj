using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GuardMovement : MonoBehaviour
{
    public float movementSpeed = 2f;
    [SerializeField] private float destructionRange = 10.0f; // Adjustable destruction range
    private Vector2 movementDirection;
    private Rigidbody2D rb;

    [SerializeField]
    private CountdownTimer countdownTimer;
    private Animator animator; // Reference to Animator
    private bool isMoving; // Boolean to track movement

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
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>(); // Initialize the Animator
        StartCoroutine(ChangeDirectionRoutine());
        countdownTimer = FindObjectOfType<CountdownTimer>();
    }

    void Update()
    {
        Move();
        UpdateAnimator();
    }

    void Move()
    {
        rb.velocity = movementDirection * movementSpeed;
    }

    void UpdateAnimator()
    {
        isMoving = rb.velocity != Vector2.zero;
        animator.SetBool("isMoving", true);
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            ChangeDirection();
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    void ChangeDirection()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        do
        {
            movementDirection = directions[Random.Range(0, directions.Length)];
        } while (IsWallInDirection());
    }

    bool IsWallInDirection()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, movementDirection, 1f);
        return hit.collider != null && hit.collider.CompareTag("Wall");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            countdownTimer.SetCountdownToZero();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            ChangeDirection();
        }
    }

    void HandleMineExploded(Vector2 minePosition)
    {
        Debug.Log("EXPLODED near guard");
        float distanceToMine = Vector2.Distance(transform.position, minePosition);
        Debug.Log("Distance to mine: " + distanceToMine);

        if (distanceToMine <= destructionRange)
        {
            Debug.Log("Guard within destruction range, destroying.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Guard out of destruction range.");
        }
    }

    // Show the destruction range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destructionRange);
    }
}
