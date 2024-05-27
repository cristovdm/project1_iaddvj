using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIInventoryItem : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text quantityTxt;
    [SerializeField] private Image borderImage;

    public event Action<UIInventoryItem> OnItemClicked,
    OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

    private bool empty = true;
    private bool hasReloaded = false;

    private void Awake()
    {
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(false);
            empty = true;
        }
    }

    public void Select()
    {
        if (borderImage != null)
        {
            borderImage.enabled = true;
        }
    }

    public void Deselect()
    {
        if (borderImage != null)
        {
            borderImage.enabled = false;
        }
    }

    public void SetData(Sprite sprite, int quantity)
    {
        if (itemImage != null)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            quantityTxt.text = quantity.ToString();
            empty = false;
        }
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        if (itemImage == null) return;

        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            HandleRightMouseClick();
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }

    private void HandleRightMouseClick()
    {
        if (itemImage != null)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemImage != null)
        {
            OnItemEndDrag?.Invoke(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemImage == null || empty) return;

        OnItemBeginDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (itemImage != null)
        {
            OnItemDroppedOn?.Invoke(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Implement if needed
    }
}
