using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScheduledSound
{
    public AudioClip clip;
    public float time;
}

public class SoundScheduler : MonoBehaviour
{
    [SerializeField]
    private List<ScheduledSound> scheduledSounds;

    [SerializeField]
    private AudioSource audioSource;

    private float elapsedTime = 0f;
    private int currentSoundIndex = 0;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        scheduledSounds.Sort((x, y) => x.time.CompareTo(y.time));

        StartCoroutine(PlayScheduledSounds());
    }

    private IEnumerator PlayScheduledSounds()
    {
        while (currentSoundIndex < scheduledSounds.Count)
        {
            ScheduledSound currentSound = scheduledSounds[currentSoundIndex];

            while (elapsedTime < currentSound.time)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            audioSource.PlayOneShot(currentSound.clip);

            currentSoundIndex++;
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public void ResetScheduler()
    {
        StopAllCoroutines();
        elapsedTime = 0f;
        currentSoundIndex = 0;
        StartCoroutine(PlayScheduledSounds());
    }
}
