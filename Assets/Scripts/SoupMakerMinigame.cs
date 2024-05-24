using UnityEngine;
using TMPro;
using System.Collections;

public class SoupMakerMiniGame : MonoBehaviour
{
    public GameObject ParentObject;
    public TextMeshProUGUI instructionText;
    public GameObject upKeySprite;
    public GameObject downKeySprite;
    public GameObject leftKeySprite;
    public GameObject rightKeySprite;

    public int totalKeySequences = 4; //Cantidad total de sequencias a completar

    private int totalKeyPairs;
    private int nextKeyPress = 0; // 0 => UP; 1 => RIGHT; 2 => DOWN; 3 => LEFT
    private bool gameActive = false;
    private bool isPlayerLocked = true;
    private bool readyToStart = true;
    private bool win = false;
    public AudioClip bubble;
    private AudioSource audioSource;
    public BoxCollider2D interactionArea;
    private bool hasStartedMiniGame = false;
    private bool isCooldown = false;
    public PlayerMovement playerMovement;

    void Start()
    {
        SetChildrenActive(ParentObject, false);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (interactionArea == null)
        {
            interactionArea = GetComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        if (isCooldown)
        {
            return;
        }

        // Disable player movement when the mini-game is active
        if (gameActive)
        {
            playerMovement.enabled = false;
        }
        else
        {
            playerMovement.enabled = true;
        }

        if (gameActive && !isPlayerLocked && !win)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                HandleKeyPress(0, false);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                HandleKeyPress(1, false);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                HandleKeyPress(2, false);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                HandleKeyPress(3, true);
            }
        }
        else
        {
            if (!IsGameActive() && !hasStartedMiniGame && Input.GetKeyDown(KeyCode.E) && IsReadyToStart())
            {
                StartMiniGame();
            }
        }
    }

    void HandleKeyPress(int keyIndex, bool endSequence)
    {
        if (nextKeyPress == keyIndex)
        {

            audioSource.PlayOneShot(bubble);
            nextKeyPress = (nextKeyPress + 1);
            ToggleKeySprite(nextKeyPress);
            if (endSequence)
            {
                nextKeyPress = 0;
                totalKeyPairs++;
                ToggleKeySprite(nextKeyPress);
                if (totalKeyPairs >= totalKeySequences)
                {
                    upKeySprite.SetActive(false);
                    downKeySprite.SetActive(false);
                    leftKeySprite.SetActive(false);
                    rightKeySprite.SetActive(false);
                    win = true;
                    instructionText.text = "COMPLETE";
                    Invoke("EndMiniGame", 1.0f);
                    //StartCoroutine(EndMiniGameAfterDelay());
                }
            }
                
        }
    }

    IEnumerator EndMiniGameAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);
        EndMiniGame();
    }

    void ToggleKeySprite(int keyIndex)
    {
        // Reset all key sprites
        upKeySprite.SetActive(false);
        downKeySprite.SetActive(false);
        leftKeySprite.SetActive(false);
        rightKeySprite.SetActive(false);

        switch (keyIndex)
        {
            case 0:
                upKeySprite.SetActive(true);
                break;
            case 1:
                rightKeySprite.SetActive(true);
                break;
            case 2:
                downKeySprite.SetActive(true);
                break;
            case 3:
                leftKeySprite.SetActive(true);
                break;
        }
    }

    void StartCooldown()
    {
        isCooldown = true;
        StartCoroutine(ResetCooldown());
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(3f);
        isCooldown = false;
    }

    void SetChildrenActive(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

    public void StartMiniGame()
    {
        hasStartedMiniGame = true;
        SetChildrenActive(ParentObject, true);
        ResetKeySprites();
        instructionText.text = "Press UP, RIGHT, DOWN, LEFT in order!";
        StartCoroutine(CountdownToStart());
    }

    IEnumerator CountdownToStart()
    {
        yield return new WaitForSeconds(1f);
        instructionText.text = "";
        upKeySprite.SetActive(true);
        gameActive = true;
        isPlayerLocked = false;
    }

    private void ResetGame()
    {
        hasStartedMiniGame = false;
        totalKeyPairs = 0;
        nextKeyPress = 0; // 0 => UP; 1 => RIGHT; 2 => DOWN; 3 => LEFT
        gameActive = false;
        isPlayerLocked = true;
        readyToStart = false;
        win = false;
    }

    void EndMiniGame()
    {
        ResetGame();
        ResetKeySprites();
        SetChildrenActive(ParentObject, false);
        StartCooldown();
        playerMovement.enabled = true;
    }

    void ResetKeySprites()
    {
        upKeySprite.SetActive(false);
        downKeySprite.SetActive(false);
        leftKeySprite.SetActive(false);
        rightKeySprite.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            readyToStart = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            readyToStart = false;
        }
    }

    public bool IsReadyToStart()
    {
        if (interactionArea == null)
        {
            Debug.LogError("Interaction Area has not been assigned in the inspector!");
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll(interactionArea.bounds.center, interactionArea.bounds.size, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}
