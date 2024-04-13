using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickIngredient : MonoBehaviour
{
    public AudioClip pickUpSound;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickUpSound != null)
            {
                audioSource.PlayOneShot(pickUpSound);
                HideSprite();
                StartCoroutine(DestroyAfterSound());
            }
        }
    }

    void HideSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false; // Hide the sprite
        }
    }

    IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(pickUpSound.length); // Wait for the sound to finish playing
        Destroy(gameObject);
    }
}
