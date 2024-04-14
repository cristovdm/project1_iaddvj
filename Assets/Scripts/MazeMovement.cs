using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMovement : MonoBehaviour
{
    public float movementSpeed;
    public float slideForce = 1f;
    float speedX, speedY;
    Vector2 lastMovement;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    private Animator anim;
    public bool isSliding = false;
    public bool isBoosted = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on player GameObject.");
        }
    }

    void Update()
    {
        speedX = Input.GetAxisRaw("Horizontal");
        speedY = Input.GetAxisRaw("Vertical");

        // Setting isBoosted parameter in Animator
        anim.SetBool("isBoosted", isBoosted);

        if (speedX != 0 || speedY != 0)
        {
            lastMovement = new Vector2(speedX, speedY).normalized;
            rb.velocity = lastMovement * movementSpeed;

            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        if (speedX > 0) // Moving right
        {
            spriteRenderer.flipX = true;
        }
        else if (speedX < 0) // Moving left
        {
            spriteRenderer.flipX = false;
        }

        // Slide
        if (speedX == 0 && speedY == 0 && isSliding)
        {
            rb.AddForce(lastMovement * slideForce, ForceMode2D.Force);
            anim.SetBool("isSliding", true);
        }
        else if (!isSliding)
        {
            rb.velocity *= 0.8f;
            anim.SetBool("isSliding", false);
        }
    }

    public void StopMovement(float stopTime)
    {
        StartCoroutine(StopMovementCoroutine(stopTime));
    }

    IEnumerator StopMovementCoroutine(float stopTime)
    {
        float originalSpeed = movementSpeed;
        movementSpeed = 0f; // Stop the player movement

        yield return new WaitForSeconds(stopTime);

        movementSpeed = originalSpeed; // Restore original movement spee
        // Reset isSliding to false
        isSliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity *= 0.8f;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isSliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isSliding = false;
        }
    }
}
