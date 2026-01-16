using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private bool hasItem = false;
    private bool hasBag = false;
    
    public int gridX;
    public int gridY;

    public Vector2Int ItemGridPos;

    public Piece bagObj;
    public Piece itemObj;
    
    public bool HasItem
    {
        get => hasItem;
        set => hasItem = value;
    }

    public bool HasBag
    {
        get => hasBag;
        set => hasBag = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (bagObj == null) return;

        bagObj.gameObject.SetActive(true);
        bagObj.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        bagObj.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bagObj.OnEndDrag(eventData);
    }

    public void SetHighlight(bool canPlace)
    {
        Image image = GetComponent<Image>();
        image.color = canPlace ? Color.green : Color.red;
    }
}