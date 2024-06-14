using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashInventory : MonoBehaviour
{
    public GameObject inventoryPrefab;
    private GameObject inventoryInstance;
    public BoxCollider2D interactionArea;
    public GameObject ParentObject;
    bool currentState = false;
    public PlayerMovement playerMovement;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;

    private float originalMovementSpeed;
    private float originalSpeedX;
    private float originalSpeedY;

    void Start()
    {
        SetChildrenActive(ParentObject, false);
        if (interactionArea == null)
        {
            interactionArea = GetComponent<BoxCollider2D>();
        }
        if (playerMovement != null)
        {
            originalMovementSpeed = playerMovement.movementSpeed;
            originalSpeedX = playerMovement.speedX;
            originalSpeedY = playerMovement.speedY;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerInInteractionArea())
        {
            currentState = !currentState;
            ToggleInventory();
        }

        if (currentState && !IsPlayerInInteractionArea())
        {
            currentState = false;
            ToggleInventory();
        }
        if (playerMovement != null)
        {
            if (currentState)
            {
                playerMovement.movementSpeed = 0f;
                playerMovement.speedX = 0f;
                playerMovement.speedY = 0f;
            }
            else
            {
                playerMovement.movementSpeed = originalMovementSpeed;
                playerMovement.speedX = originalSpeedX;
                playerMovement.speedY = originalSpeedY;
            }
        }
    }

    void ToggleInventory()
    {
        SetChildrenActive(ParentObject, currentState);
        if (currentState){
            audioSource.PlayOneShot(openSound);
        }
    }

    private bool IsPlayerInInteractionArea()
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

    void SetChildrenActive(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }
}
