using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float slideForce = 1f;
    public float speedX, speedY;
    Vector2 lastMovement;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    private Animator anim;
    public bool isSliding = false;
    public bool isBoosted = false;
    public bool canCollideWithBanana = true;


    public float punchDuration = 0.5f;

    public Canvas Book;

    public GameObject exitMenu;
    public Button yesButton;
    public Button noButton;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        Book.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on player GameObject.");
        }


        if (exitMenu != null)
        {
            exitMenu.SetActive(false);
            yesButton.onClick.AddListener(OnYesButtonClicked);
            noButton.onClick.AddListener(OnNoButtonClicked);
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

        // Trigger punch animation
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TriggerPunchAnimation();
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
        if (other.CompareTag("Exit"))
        {
            ShowExitMenu();
        }
        if (other.CompareTag("Book"))
        {
            Book.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            isSliding = false;
        }
        if (other.CompareTag("Book"))
        {
            Book.enabled = false;
        }
    }

    public void StartBananaCooldown(float cooldownTime)
    {
        StartCoroutine(BananaCooldownCoroutine(cooldownTime));
    }

    IEnumerator BananaCooldownCoroutine(float cooldownTime)
    {
        canCollideWithBanana = false;
        yield return new WaitForSeconds(cooldownTime);
        canCollideWithBanana = true;
    }

    private void TriggerPunchAnimation()
    {
        anim.SetBool("isPunching", true);
        StartCoroutine(ResetPunchAnimation());
    }

    IEnumerator ResetPunchAnimation()
    {
        yield return new WaitForSeconds(punchDuration);
        anim.SetBool("isPunching", false);
    }

    private void ShowExitMenu()
    {
        if (exitMenu != null)
        {
            exitMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void HideExitMenu()
    {
        if (exitMenu != null)
        {
            exitMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void OnYesButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Maze");
    }

    private void OnNoButtonClicked()
    {
        HideExitMenu();
    }

}