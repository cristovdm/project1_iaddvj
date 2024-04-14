using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CuttingBoardMiniGame : MonoBehaviour
{
    public Text timerText;
    public Text instructionText;
    public float gameTime = 7f;
    public KeyCode[] requiredKeys; 
    private int keyIndex = 0; 
    private bool gameActive = false;
    private bool readyToStart = true;  // Keeping it hardcoded as true for now
    public int totalKeyPresses = 10; 
    private int keyPresses = 0; 
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
                Debug.Log("Pressed W");
                keyPresses++;
                keyIndex++;
            }
            else if (Input.GetKeyDown(KeyCode.S) && keyIndex < totalKeyPresses && keyIndex % 2 != 0)
            {
                Debug.Log("Pressed S");
                keyPresses++;
                keyIndex++;
            }

            if (keyPresses >= totalKeyPresses)
            {
                instructionText.text = "Mini-game completed!";
                EndMiniGame();
            }
        }
        else
        {
            Debug.Log("Game not active or player is locked.");
        }
    }

    public void StartMiniGame()
    {
        Debug.Log("StartMiniGame method called.");
        instructionText.text = "Get ready...";
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        isPlayerLocked = true;  // Lock the player during countdown
        yield return new WaitForSeconds(1f);
        instructionText.text = "3";
        yield return new WaitForSeconds(1f);
        instructionText.text = "2";
        yield return new WaitForSeconds(1f);
        instructionText.text = "1";
        yield return new WaitForSeconds(1f);
        instructionText.text = "Go! press W and S in order repeatedly!";
        gameActive = true;
        isPlayerLocked = false;  // Unlock the player after countdown
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
        instructionText.text = "Time's up!";
        EndMiniGame();
    }

    void EndMiniGame()
    {
        gameActive = false;
        isPlayerLocked = false;  // Ensure player is unlocked
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
