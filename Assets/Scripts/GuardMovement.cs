using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardMovement : MonoBehaviour
{
    public float movementSpeed = 2f;
    private Vector2 movementDirection;
    private Rigidbody2D rb;

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
            SceneManager.LoadScene("Kitchen");
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            ChangeDirection();
        }
    }
}
