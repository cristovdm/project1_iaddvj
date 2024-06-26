using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private AudioClip collectionSound;
    [SerializeField] private float soundVolume = 1.0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Money.Instance.AddMoney(coinValue);
            PlaySound();
            Destroy(gameObject);
        }
    }

    private void PlaySound()
    {
        if (collectionSound != null)
        {
            GameObject tempAudioSource = new GameObject("CoinCollectionSound");
            tempAudioSource.transform.position = transform.position;

            AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
            audioSource.clip = collectionSound;
            audioSource.volume = soundVolume;
            audioSource.Play();

            Destroy(tempAudioSource, collectionSound.length);
        }
    }
}
