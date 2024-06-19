using UnityEngine;
using System.Collections.Generic;

public class Crate : MonoBehaviour
{
    public int hitsToDestroy = 3;
    private int currentHits = 0;
    private bool playerInRange = false;
    private bool isDestroyed = false;

    [SerializeField]
    private BoxCollider2D triggerCollider;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip hitSound;

    [SerializeField]
    private AudioClip crateDestroy;

    [SerializeField]
    private List<GameObject> dropPrefabs; // Lista de prefabs a soltar al destruirse

    private Vector3 originalPosition;
    private bool isShaking = false;

    private void Awake()
    {
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<BoxCollider2D>();
        }

        if (triggerCollider != null && !triggerCollider.isTrigger)
        {
            Debug.LogWarning("El Collider asignado no es un trigger. Cambiando a trigger.");
            triggerCollider.isTrigger = true;
        }
        originalPosition = transform.localPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger zone.");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger zone.");
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q) && !isDestroyed)
        {
            currentHits++;
            PlayHitSound();
            StartCoroutine(ShakeCrate(0.1f, 0.3f));

            if (currentHits >= hitsToDestroy)
            {
                StartCoroutine(WaitAndDestroyCrate());
            }
        }
    }

    void PlayHitSound()
    {
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
        else
        {
            Debug.LogWarning("AudioSource o hitSound no están asignados.");
        }
    }

    System.Collections.IEnumerator ShakeCrate(float duration, float magnitude)
    {
        if (isShaking) yield break;

        isShaking = true;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
        isShaking = false;
    }

    System.Collections.IEnumerator WaitAndDestroyCrate()
    {
        isDestroyed = true;
        PlayDestroySound();

        yield return new WaitForSeconds(0.3f);

        float randomValue = Random.Range(0f, 1f);
        if (randomValue > 0.8f && dropPrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, dropPrefabs.Count);
            GameObject prefabToDrop = dropPrefabs[randomIndex];
            Instantiate(prefabToDrop, transform.position, Quaternion.identity);
        }

        Debug.Log("Crate destroyed!");
        Destroy(gameObject);
    }

    void PlayDestroySound()
    {
        if (crateDestroy != null)
        {
            GameObject audioObject = new GameObject("CrateDestroyAudio");
            AudioSource tempAudioSource = audioObject.AddComponent<AudioSource>();
            tempAudioSource.clip = crateDestroy;
            tempAudioSource.volume = 0.6f;
            tempAudioSource.Play();

            Destroy(audioObject, crateDestroy.length);
        }
        else
        {
            Debug.LogWarning("CrateDestroy no está asignado.");
        }
    }
}
