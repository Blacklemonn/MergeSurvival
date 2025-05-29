using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public bool isOccupied = false;
    public Image itemIcon;

    public void AddItem(Sprite icon)
    {
        isOccupied = true;
        itemIcon.sprite = icon;
        itemIcon.enabled = true;
    }

    public void ClearSlot()
    {
        isOccupied = false;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }
}