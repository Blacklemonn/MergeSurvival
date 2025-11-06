using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentUI : Piece
{
    public ItemData itemData;

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
            //현재 가져온 slot과 활성화 시키려는 slot이 같으면 ReturnToOriginalSlot을 실행
            if (slot == inventoryManager.slots[gridPos.x, gridPos.y].GetComponent<InventorySlot>())
            {
                ReturnToOriginalSlot(item, slot);
                return;
            }

            //조각의 이미지 가방 활성화
            inventoryManager.PlacePiece(gridPos, itemData, gameObject);

            //슬롯의 중앙에서 얼마나 떨어진 위치를 드래그 했는지 가져오고 현재 둔 슬롯의 중앙에서 얼마나 떨어진지 위치를 가져와서 두 거리의 차이를 계산

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
