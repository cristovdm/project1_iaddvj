using System.Collections;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public float stopTime = 1f;
    public float disappearDelay = 0.1f;
    public float cooldownTime = 1f;
    public AudioClip slipSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            if (playerMovement != null && playerMovement.canCollideWithBanana)
            {
                playerMovement.isSliding = true;
                playerMovement.StopMovement(stopTime);
                playerMovement.StartBananaCooldown(cooldownTime);

                StartCoroutine(DisappearAfterDelay(disappearDelay));
            }
        }
    }

    IEnumerator DisappearAfterDelay(float delay)
    {
        if (slipSound != null)
        {
            audioSource.PlayOneShot(slipSound);
        }

        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}