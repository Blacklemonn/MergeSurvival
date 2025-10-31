using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BagPieceUI : Piece
{
    public BagPieceData pieceData;

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

    public override void ReturnToOriginalSlot(Vector2Int gridPos, GameObject item, InventorySlot slot)
    {
        if (inventoryManager.CanSlotActivate(gridPos, pieceData.width, pieceData.height))
        {
            //조각의 이미지 가방 활성화
            inventoryManager.ActiveSlots(gridPos, gameObject);
        }
        else
        {
            //조각의 이미지 가방 활성화
            slot.stackItem(item);
        }
    }
}
