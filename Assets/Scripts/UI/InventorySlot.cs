using Unity.VisualScripting;
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

    public void CheckMerge()
    {
        InventorySlot sl;
        //왼쪽 슬롯확인
        //sl = GameManager.instance.inventory.grid[gridX - 1, gridY];
        //CheckSlot(sl);
        //오른쪽 슬롯 확인
        //위쪽 슬롯 확인
        //아래쪽 슬롯 확인
    }

    private void CheckSlot(InventorySlot sl)
    {
        //슬롯의 근처에 아이템이 있을때만 실행
        if (!sl.hasItem) return;
        //슬롯의 근처 아이템이 들고있는 아이템 오브젝트와 같은 오브젝트일 경우 return;
        if (itemObj == sl.itemObj) return;
        //슬롯의 근처 아이템이 합쳐질수 있는 상태인지 확인 후 불가능하면 return;
        //if (!sl.itemObj.canMerge) return;
        //슬롯의 근처 슬롯이 갖고있는 아이템의 itemdata에서 합쳐지는 변수인지 검사
        for (int i = 0; i < itemObj.itemData.resultRecipe.Length; i++)
        {
            //itemObj.itemData.mergeType[i]로 합쳐지기 위한 필요 타입을 검사
            //foreach (ItemData.ItemType type in itemObj.itemData.mergeType[i])
            //{
            //    if (!sl.itemObj.canMerge)
            //        return;
            //    if (type == sl.itemObj.itemData.itemType)
            //    {

            //    }
            //}
        }
        //없으면 반복하고 넘어가기 있으면 반복문 빠져나가고 5번
        //슬롯의 item합쳐지는 아이템과 이 아이템 둘다 합쳐질 수 있는 상태가 아니게 바꾸기
        //아이템 piece에 합쳐질 아이템 정보 저장

        //7.게임 매니저에서 스테이지가 끝났을때
        //8.합쳐지는 함수 호출해서 합치기 시작

        //9.코루틴으로 합쳐질 아이템의 위치를 이 아이템의 위치로 이동 후 합쳐질 아이템 제거
        //->기존에 들고있던 아이템 remove
        //10.아이템의 itemdata를 합쳐지면 되는 itemdata값으로 변경
        //->itemdata apply
        //11.아이템 리사이즈함수 호출
        //12.스타트 슬롯 기준으로 아이템 둘 수 있는지 확인
        //13.가능하면 두고 불가능하면 창고로 이동
    }
}