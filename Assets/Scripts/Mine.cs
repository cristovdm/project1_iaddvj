using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Mine : MonoBehaviour
{
    [SerializeField] private float activeDuration = 2f;
    [SerializeField] private float beepDuration = 1.5f;
    [SerializeField] private Light2D mineLight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip beepSound;
    [SerializeField] private Animator animator;
    [SerializeField] private float proximityThreshold = 2f; // Proximidad para el cambio de escena
    [SerializeField] private float checkInterval = 0.5f; // Intervalo entre comprobaciones de proximidad

    private bool isActive = false;
    private CircleCollider2D mineCollider;

    void Start()
    {
        mineCollider = GetComponent<CircleCollider2D>();

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
                    yield break; // Termina el bucle si se cumple la condición
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

        // Destruir objetos en el rango con tag "Food" y "Guard"
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, mineCollider.radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Food") || hitCollider.CompareTag("Guard"))
            {
                Destroy(hitCollider.gameObject);
            }
        }

        // Calcular la distancia al jugador y cambiar de escena si está dentro del umbral
        if (player != null && player.CompareTag("Player"))
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= proximityThreshold)
            {
                SceneManager.LoadScene("Kitchen");
            }
        }

        Destroy(gameObject);
    }

    // Este método será llamado al final de la animación de explosión.
    public void OnExplosionAnimationEnd()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
