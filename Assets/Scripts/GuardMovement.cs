using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GuardMovement : MonoBehaviour
{
    public float movementSpeed = 2f;
    [SerializeField] private float destructionRange = 10.0f; // Rango de destrucci�n ajustable
    private Vector2 movementDirection;
    private Rigidbody2D rb;
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
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        rb.velocity = movementDirection * movementSpeed;
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
            if (canvasChangeScene != null)
            {
                canvasChangeScene.SetActive(true); 
            }
            else
            {
                Debug.LogError("Canvas Change Scene not found");
            }
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

    // Mostrar el rango de destrucci�n en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destructionRange);
    }

}
