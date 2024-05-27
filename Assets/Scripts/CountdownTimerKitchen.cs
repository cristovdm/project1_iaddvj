using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimerKitchen : MonoBehaviour
{
    [SerializeField]
    private float countdownTime = 60f;

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
                currentTime = 0;
                timeUp = true;
                timerText.text = "TIMES UP! Press E to continue";

                if (playerMovementScript != null)
                {
                    playerMovementScript.enabled = false;
                }

                if (audioSource != null)
                {
                    audioSource.volume = 0;
                }
            }
            else
            {
                int minutes = Mathf.FloorToInt(currentTime / 60F);
                int seconds = Mathf.FloorToInt(currentTime % 60F);
                timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene("Maze");
            }
        }
    }
}
