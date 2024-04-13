using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CuttingBoardMiniGame : MonoBehaviour
{
    public Text timerText;
    public Text instructionText;
    public float gameTime = 7f;
    public KeyCode[] requiredKeys; // Keys to be pressed in sequence
    private int keyIndex = 0; // Index of the currently required key
    private bool gameActive = false;
    private bool readyToStart = true;
    public int totalKeyPresses = 10; // Total key presses required
    private int keyPresses = 0; // Counter for key presses
    private bool isPlayerLocked = false;

    void Start()
    {
        instructionText.text = "Press 'E' to start the mini-game.";
    }

    void Update()
    {
        if (gameActive && !isPlayerLocked)
        {
            if (Input.GetKeyDown(KeyCode.W) && keyIndex < totalKeyPresses && keyIndex % 2 == 0)
            {
                keyPresses++;
                keyIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.S) && keyIndex < totalKeyPresses && keyIndex % 2 != 0)
            {
                keyPresses++;
                keyIndex++;
            }

            if (keyPresses >= totalKeyPresses)
            {
                instructionText.text = "Mini-game completed!";
                EndMiniGame();
            }
        }
    }

    public void StartMiniGame()
    {
        instructionText.text = "Get ready...";
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(1f);
        instructionText.text = "3";
        yield return new WaitForSeconds(1f);
        instructionText.text = "2";
        yield return new WaitForSeconds(1f);
        instructionText.text = "1";
        yield return new WaitForSeconds(1f);
        instructionText.text = "Go! press W and S in order repeatedly!";
        gameActive = true;
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        float timer = gameTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("F1");
            yield return null;
        }
        // Game over if timer runs out
        instructionText.text = "Time's up!";
        EndMiniGame();
    }

    void EndMiniGame()
    {
        gameActive = false;
        // Add any additional logic for ending the mini-game here
    }
    public bool IsReadyToStart()
    {
        return readyToStart;
    }
    public bool IsGameActive()
    {
        return gameActive;
    }
}
