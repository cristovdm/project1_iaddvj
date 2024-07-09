using UnityEngine;

public class RandomSongPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        if (songs.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, songs.Length);
            AudioClip randomSong = songs[randomIndex];
            audioSource.clip = randomSong;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No songs or AudioSource assigned.");
        }
    }
}
