using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections; // Importa el espacio de nombres para usar Coroutine
using System.Linq;

public class CountdownTimer : MonoBehaviour
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

    private GameObject canvasChangeScene;

    void OnEnable()
    {
        canvasChangeScene = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == "CanvasChangeScene");
    }

    void Start()
    {
        currentTime = countdownTime;

        // Obt�n el componente PlayerMovement del jugador
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
                timerText.text = "TIMES UP!";

                // Pausa el juego
                Time.timeScale = 0;

                // Desactiva el script de movimiento del jugador
                if (playerMovementScript != null)
                {
                    playerMovementScript.enabled = false;
                }

                // Silencia el audio si est� asignado
                if (audioSource != null)
                {
                    audioSource.volume = 0;
                }

                // Inicia la rutina para cambiar la escena despu�s de 3 segundos
                StartCoroutine(ChangeSceneAfterDelay(0.5f));
            }
            else
            {
                // Actualiza el texto del temporizador
                int minutes = Mathf.FloorToInt(currentTime / 60F);
                int seconds = Mathf.FloorToInt(currentTime % 60F);
                timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }
        }
    }

    private IEnumerator ChangeSceneAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Espera en tiempo real, no afectado por Time.timeScale
        Money moneyComponent = canvasChangeScene.GetComponent<Money>();
        if (moneyComponent != null)
        {
            moneyComponent.UpdateAllUI();
            canvasChangeScene.SetActive(true);
        }
        else
        {
            Debug.LogError("Money component not found on the GameObject.");
        }
        Time.timeScale = 1; // Restablece el tiempo para la nueva escena
    }
}
