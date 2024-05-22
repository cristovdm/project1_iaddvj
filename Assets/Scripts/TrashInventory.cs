using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrashInventory : MonoBehaviour
{
    public GameObject inventoryPrefab;  // Prefab del inventario
    private GameObject inventoryInstance; // Instancia del inventario
    public BoxCollider2D interactionArea; // Área de interacción
    public GameObject ParentObject;
    bool currentState = false;
    public PlayerMovement playerMovement;

    private bool isPlayerInRange = false;

    void Start()
    {   
        SetChildrenActive(ParentObject, false);
        if (interactionArea == null)
        {
            interactionArea = GetComponent<BoxCollider2D>();
        }
    }

    void Update()
    {
        // Si el jugador está en el área y presiona 'E', mostramos/ocultamos el inventario
        if (Input.GetKeyDown(KeyCode.E) && IsReadyToStart())
        {
            playerMovement.enabled = currentState; // deberia desactivar el movimiento mientras ve el inventario, revisar porque no funciona
            currentState = !currentState;
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {  
        SetChildrenActive(ParentObject, currentState);
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

    void SetChildrenActive(GameObject parent, bool state)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(state);
        }
    }
}
