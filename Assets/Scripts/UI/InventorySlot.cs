using System.Collections.Generic;
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

    public void NearSlots(List<InventorySlot> slots)
    {
        InventorySlot sl;
        //왼쪽 슬롯확인
        if (gridX - 1 > 0)
        {
            sl = GameManager.instance.inventory.grid[gridX - 1, gridY];
            if (CheckSlot(sl))
                slots.Add(sl);
        }
        //오른쪽 슬롯 확인
        if (gridX + 1 < 5)
        {
            sl = GameManager.instance.inventory.grid[gridX + 1, gridY];
            if (CheckSlot(sl))
                slots.Add(sl);
        }
        //위쪽 슬롯 확인
        if (gridY + 1 < 5)
        {
            sl = GameManager.instance.inventory.grid[gridX, gridY + 1];
            if (CheckSlot(sl))
                slots.Add(sl);
        }
        //아래쪽 슬롯 확인
        if (gridY - 1 > 0)
        {
            sl = GameManager.instance.inventory.grid[gridX, gridY - 1];
            if (CheckSlot(sl))
                slots.Add(sl);
        }
    }

    private bool CheckSlot(InventorySlot sl)
    {
        //나중에 아이템의 가로세로를 받아서 최적화할 수 있게
        //슬롯의 근처 아이템이 들고있는 아이템 오브젝트와 같은 오브젝트일 경우 return;
        if (itemObj == sl.itemObj) return false;
        //슬롯의 근처에 아이템이 있을때만 실행
        if (sl.itemObj == null) return false;
        
        return true;
    }
}