using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField]
    private UIInventoryItem itemPrefab; 

    [SerializeField]
    private RectTransform contentPanel; 

    List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

    public void InitializeInventoryUI(int inventorysize){

        for (int i = 0; i < inventorysize; i++)
        {
            UIInventoryItem uiItem =
                Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(contentPanel);
            listOfUIItems.Add(uiItem);

            /*
            uiItem.OnItemClicked += HandleItemSelection;
            uiItem.OnItemBeginDrag += HandleBeginDrag;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            */
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        // ResetSelection();
    }

    public void Hide()
    {
        // actionPanel.Toggle(false);
        gameObject.SetActive(false);
        // ResetDraggedItem();
    }

}
