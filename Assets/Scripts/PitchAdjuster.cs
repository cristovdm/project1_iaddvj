using UnityEngine;

public class PitchAdjuster : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private float pitchIncreaseAmount = 0.1f;

    [SerializeField]
    private float adjustInterval = 5f;

    private float timeSinceLastAdjust = 0f;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.pitch = 1f;
    }

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned.");
        }
    }

    void Update()
    {
        if (audioSource == null)
            return;
        timeSinceLastAdjust += Time.deltaTime;
        if (timeSinceLastAdjust >= adjustInterval)
        {
            audioSource.pitch += pitchIncreaseAmount;
            timeSinceLastAdjust = 0f;
        }
    }
}
