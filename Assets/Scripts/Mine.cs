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
            // Comprobar si el jugador está en el rango de la mina al activarse
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, mineCollider.radius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    StartCoroutine(BeepAndExplode(hitCollider.gameObject));
                    break;
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

        // Destruir objetos en el rango con tag "Food", "Guard", y "Wall"
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, mineCollider.radius);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider);
            if (hitCollider.CompareTag("Food") || hitCollider.CompareTag("Guard"))
            {
                Destroy(hitCollider.gameObject);
            }
        }

        // Verificar si el jugador está en el rango después de la explosión
        if (player != null && player.CompareTag("Player") && mineCollider.IsTouching(player.GetComponent<Collider2D>()))
        {
            SceneManager.LoadScene("Kitchen");
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
