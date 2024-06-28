using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CountdownTimerKitchen : MonoBehaviour
{
    [SerializeField]
    private float countdownTime = 120f;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private AudioSource audioSource;

    private float currentTime;
    private bool timeUp = false;
    private PlayerMovement playerMovementScript;

    void Start()
    {
        currentTime = countdownTime;
        playerMovementScript = player.GetComponent<PlayerMovement>();

        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource no asignado en el CountdownTimer.");
        }
    }

    void Update()
    {
        if (!timeUp)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                TimeUp();
            }
            else
            {
                UpdateTimerText();
            }
        }
    }

    public void SetCountdownToZero()
    {
        currentTime = 0;
        TimeUp();
    }

    private void TimeUp()
    {
        currentTime = 0;
        timeUp = true;
        timerText.text = "TIMES UP!";

        Time.timeScale = 0;

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (audioSource != null)
        {
            audioSource.volume = 0;
        }

        StartCoroutine(ChangeSceneAfterDelay(0.5f));
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    private IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LevelManager.instance.NextLevel();
        SceneManager.LoadScene("Maze");
        Time.timeScale = 1;
    }
}