using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BagPieceUI : Piece
{
    public BagPieceData pieceData;

    //샵이나 창고에 있는 아이템을 옮길때 사용되는 함수
    public override void Activeslot(Vector2Int gridPos)
    {
        if (inventoryManager.CanSlotActivate(gridPos, pieceData.width, pieceData.height))
        {
            //조각의 이미지 가방 활성화
            inventoryManager.ActiveSlots(gridPos, gameObject);
        }
        else
        {
            Debug.Log("인벤토리 밖에 존재함");
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    //인벤토리 슬롯에 있는 아이템을 옮길때 사용되는 함수
    public override void Activeslot(Vector2Int gridPos, GameObject item, InventorySlot slot)
    {

        if (inventoryManager.CanSlotActivate(gridPos, pieceData.width, pieceData.height))
        {
            //현재 가져온 slot과 활성화 시키려는 slot이 같으면 ReturnToOriginalSlot을 실행
            if (slot == inventoryManager.slots[gridPos.x, gridPos.y].GetComponent<InventorySlot>())
            {
                ReturnToOriginalSlot(item, slot);
                return;
            }

            //조각의 이미지 가방 활성화
            inventoryManager.ActiveSlots(gridPos, gameObject);

        }
        else
        {
            Debug.Log("인벤토리 밖에 존재함");
            ReturnToOriginalSlot(item, slot);
        }
    }

    public void ReturnToOriginalSlot(GameObject item, InventorySlot slot)
    {
        Debug.Log("Return");
        slot.ActiveInventory();
        slot.stackItem(item);
        item.transform.SetParent(inventoryManager.itemStorage.transform);
        item.GetComponent<RectTransform>().anchoredPosition = slot.originAnchorPos;
    }
}
