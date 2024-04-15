using UnityEngine;
using TMPro;
using System.Collections;
using System.Threading.Tasks;

public class CuttingBoardMiniGame : MonoBehaviour
{
    public GameObject ParentObject;
    public TextMeshProUGUI instructionText;
    public GameObject wKeySprite;
    public GameObject sKeySprite;
    public int totalKeyPresses = 10;
    private int totalKeyPairs;
    private int keyPresses = 0;
    private int nextKeyPress = 0; // 0 => W; 1 => S
    private bool gameActive = false;
    private bool isPlayerLocked = true;
    private bool readyToStart = true;  // Keeping it hardcoded as true for now
    private bool win = false;

    void Start()
    {
        //StartMiniGame();
        SetChildrenActive(ParentObject, false);
    }

    void Update()
    {
        if (gameActive && !isPlayerLocked)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Pressed W");
                if (nextKeyPress == 0)
                {
                    //keyPresses++;
                    ToggleKeySprite();
                    nextKeyPress = 1;
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Pressed S");
                if (nextKeyPress == 1)
                {
                    keyPresses++;
                    ToggleKeySprite();
                    nextKeyPress = 0;
                }
            }

            if (keyPresses >= totalKeyPairs)
            {
                //instructionText.text = "Mini-game completed!";
                win = true;
                instructionText.text = "SUCCESS";
                wKeySprite.SetActive(false);
                sKeySprite.SetActive(false);
                //Entregar el item al personaje o algo asi
                Invoke("EndMiniGame", 2.0f);
            }
        }
    }

    void SetChildrenActive(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

    void ToggleKeySprite()
    {
        wKeySprite.SetActive(!wKeySprite.activeSelf);
        sKeySprite.SetActive(!sKeySprite.activeSelf);
    }

    public void StartMiniGame()
    {
        totalKeyPairs = totalKeyPresses;
        SetChildrenActive(ParentObject, true);
        wKeySprite.SetActive(false);
        sKeySprite.SetActive(false);
        Debug.Log("StartMiniGame method called.");
        instructionText.text = "Press W and S in order repeatedly!";
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
        instructionText.text = "";
        wKeySprite.SetActive(true);
        gameActive = true;
        isPlayerLocked = false;  // Unlock the player after countdown
    }

    private void ResetGame()
    {
        totalKeyPairs = totalKeyPresses;
        keyPresses = 0;
        nextKeyPress = 0; // 0 => W; 1 => S
        gameActive = false;
        isPlayerLocked = true;
        readyToStart = true;
        win = false;
}

    void EndMiniGame()
    {
        ResetGame();
        wKeySprite.SetActive(false);
        sKeySprite.SetActive(false);
        SetChildrenActive(ParentObject, false);
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
