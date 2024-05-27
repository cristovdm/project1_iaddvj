using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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

        if (speedX > 0) 
        {
            spriteRenderer.flipX = true;
        }
        else if (speedX < 0) 
        {
            spriteRenderer.flipX = false;
        }


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
        movementSpeed = 0f; 

        yield return new WaitForSeconds(stopTime);

        movementSpeed = originalSpeed; 
      
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
