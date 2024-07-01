using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class Mine : MonoBehaviour
{
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float beepDuration = 1.5f;
    [SerializeField] private Light2D mineLight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private Animator animator;
    [SerializeField] private float proximityThreshold = 2f;
    [SerializeField] private float checkInterval = 0.5f;

    [SerializeField] private float explosionRadius = 2f;

    private bool isActive = false;
    private CircleCollider2D mineCollider;

    public static event Action<Vector2> OnMineExploded;

    [SerializeField]
    private CountdownTimer countdownTimer;


    void Start()
    {
        mineCollider = GetComponent<CircleCollider2D>();

        countdownTimer = FindObjectOfType<CountdownTimer>();

        if (mineLight == null || mineCollider == null)
        {
            Debug.LogError("Light2D or CircleCollider2D component missing.");
            return;
        }

        if (audioSource == null || explosionSound == null || beepSound == null)
        {
            Debug.LogError("AudioSource, explosion sound, or beep sound missing.");
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator component missing.");
            return;
        }

        StartCoroutine(ToggleState());
    }

    private IEnumerator ToggleState()
    {
        while (true)
        {
            SetActiveState(!isActive);
            yield return new WaitForSeconds(activeDuration);
        }
    }

    private void SetActiveState(bool state)
    {
        isActive = state;
        mineLight.enabled = isActive;

        if (isActive)
        {
            StartCoroutine(CheckProximityAndExplode());
        }
    }

    private IEnumerator CheckProximityAndExplode()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(checkInterval);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= proximityThreshold)
                {
                    StartCoroutine(BeepAndExplode(player));
                    yield break;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isActive && other.CompareTag("Player"))
        {
            StartCoroutine(BeepAndExplode(other.gameObject));
        }
    }

    private IEnumerator BeepAndExplode(GameObject player)
    {
        audioSource.PlayOneShot(beepSound);

        yield return new WaitForSeconds(beepDuration);

        if (player != null && player.CompareTag("Player"))
        {
            StartCoroutine(Explode(player));
        }
    }

    private IEnumerator Explode(GameObject player)
    {
        animator.SetTrigger("StartExplosion");
        audioSource.PlayOneShot(explosionSound);

        yield return new WaitForSeconds(0.5f);

        OnMineExploded?.Invoke(transform.position);

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= proximityThreshold)
        {
            countdownTimer.SetCountdownToZero();
        }

        Destroy(gameObject);
    }

    // Mostrar el rango de la explosión en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Este método será llamado al final de la animación de explosión.
    public void OnExplosionAnimationEnd()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
