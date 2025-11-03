using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentUI : Piece
{
    public ItemData itemData;
    private InventorySlot prevSlot;

    //샵이나 창고에 있는 아이템을 옮길때 사용되는 함수
    public override void Activeslot(Vector2Int gridPos)
    {
        //인벤토리창 조각의 위치에 둘 수 있는경우
        if (inventoryManager.CanPlacePiece(gridPos, itemData.width, itemData.height))
        {
            //조각의 이미지 가방 활성화
            inventoryManager.PlacePiece(gridPos, itemData, gameObject);
        }
        //인벤토리창 조각의 위치에 둘 수 없는경우
        else
        {
            Debug.Log("인벤토리 밖에 존재함");
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    //인벤토리 슬롯에 있는 아이템을 옮길때 사용되는 함수
    public override void Activeslot(Vector2Int gridPos, GameObject item, InventorySlot slot)
    {
        //인벤토리창 조각의 위치에 둘 수 있는경우
        if (inventoryManager.CanPlacePiece(gridPos, itemData.width, itemData.height))
        {
            //조각의 이미지 가방 활성화
            inventoryManager.PlacePiece(gridPos, itemData, gameObject);
        }
        //인벤토리창 조각의 위치에 둘 수 없는경우
        else
        {
            ReturnToOriginalSlot(item, slot);
        }
    }

    public void ReturnToOriginalSlot(GameObject item, InventorySlot slot)
    {
        slot.AddItem(itemData.itemIcon);
        slot.stackItem(item);
        item.transform.parent = inventoryManager.itemStorage.transform;
        item.GetComponent<RectTransform>().anchoredPosition = slot.originAnchorPos;
    }
}
