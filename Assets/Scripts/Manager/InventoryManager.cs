using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public int width = 5;
    public int height = 5;

    public GameObject slotPrefab;
    public Transform slotParent;
    public GameObject itemStorage;
    public GameObject tempItemStorage;

    public InventorySlot[,] slots = new InventorySlot[5, 5];

    void Awake()
    {
        slots = new InventorySlot[width, height];

        SetGridPosition();
    }

    public void SetGridPosition()
    {
        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                InventorySlot slot = slotParent.GetChild(i).GetComponent<InventorySlot>();
                slots[x, y] = slot;
                slot.gridPosition = new Vector2Int(x, y);
                i++;
            }
        }
    }

    public bool IsOccupied(int x, int y)
    {
        return slots[x, y].isOccupied;
    }

    public bool IsActive(int x, int y)
    {
        return slots[x,y].isActive;
    }

    public bool CanSlotActivate(Vector2Int gridPos, int pieceWidth, int pieceHeight)
    {
        //지금 넣는 크기를 확인하고 배치 가능한지 일일이 확인
        for (int y = 0; y < pieceHeight; y++)
        {
            for (int x = 0; x < pieceWidth; x++)
            {
                int checkX = gridPos.x + x;
                int checkY = gridPos.y + y;

                if (checkX < 0 || checkY < 0 || checkX >= width || checkY >= height || IsActive(checkX, checkY))
                    return false;
            }
        }
        return true;
    }

    public bool CanPlacePiece(Vector2Int gridPos, int pieceWidth, int pieceHeight)
    {
        //지금 넣는 크기를 확인하고 배치 가능한지 일일이 확인
        for (int y = 0; y < pieceHeight; y++)
        {
            for (int x = 0; x < pieceWidth; x++)
            {
                int checkX = gridPos.x + x;
                int checkY = gridPos.y + y;

                if (checkX < 0 || checkY < 0 || checkX >= width || checkY >= height || IsOccupied(checkX, checkY) || !IsActive(checkX, checkY))
                    return false;
            }
        }

        return true;
    }

    public void ActiveSlots(Vector2Int gridpos, GameObject item)
    {
        int i = gridpos.x + (gridpos.y * 5);
        InventorySlot slot = slotParent.GetChild(i).GetComponent<InventorySlot>();
        slot.ActiveInventory();
        slot.stackItem(item);
        item.transform.SetParent(itemStorage.transform);
    }

    public void PlacePiece(Vector2Int gridPos, ItemData itemdata, GameObject item)
    {
        for (int y = 0; y < itemdata.height; y++)
        {
            for (int x = 0; x < itemdata.width; x++)
            {
                int px = gridPos.x + x;
                int py = gridPos.y + y;
                slots[px, py].AddItem(itemdata.itemIcon);
                slots[px, py].stackItem(item);
                item.transform.parent = itemStorage.transform;
            }
        }
    }
}