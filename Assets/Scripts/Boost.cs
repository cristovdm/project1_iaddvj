using System.Collections;
using UnityEngine;

public class Boost : MonoBehaviour
{
    public AudioClip pickUpSound;
    public float boostDuration = 15f;
    public float boostMultiplier = 1.5f;

    private AudioSource audioSource;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private bool hasBeenPickedUp = false; // To track if the boost has been picked up

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogWarning("PlayerMovement script not found on any GameObject in the scene.");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            if (pickUpSound != null)
            {
                audioSource.PlayOneShot(pickUpSound);
                ApplyBoost();
                StartCoroutine(DestroyAfterDuration());

                // Hide sprite and set flag to prevent picking up again
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = false;
                }

                hasBeenPickedUp = true; // Set to true to prevent picking up again
            }
        }
    }

    void ApplyBoost()
    {
        if (playerMovement != null)
        {
            playerMovement.movementSpeed *= boostMultiplier;
            playerMovement.isBoosted = true;
        }
    }

    IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(boostDuration);
        if (playerMovement != null)
        {
            playerMovement.movementSpeed /= boostMultiplier;
            playerMovement.isBoosted = false;
        }
        Destroy(gameObject);
    }
}

