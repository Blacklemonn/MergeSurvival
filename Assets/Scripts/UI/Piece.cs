using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public ItemData itemData;

    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private RectTransform rect;
    private Transform parent;
    private int siblingIndex;
    private Sprite display;
    private Sprite panel;
    private GameObject itemTemp;
    private float slotSize;

    private bool isShop;

    [SerializeField]
    private ItemDesc itemdesc;

    private Inventory inventory;
    //테스트
    private Vector2Int offsetVector;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        itemTemp = GameManager.instance.itemTemp;
        slotSize = 12f;
    }

    private void Start()
    {
        if (itemData != null)
            ChangeItemData(itemData);

        inventory = GameManager.instance.inventory;
    }

    public void ChangeItemData(ItemData data)
    {
        itemData = data;
        //크기에 맞게 리사이징
        var size = new Vector2(itemData.width * slotSize, itemData.height * slotSize);
        rect.sizeDelta = size;
        panel = itemData.itemIcon;
        display = itemData.displayIcon;
        GetComponent<Image>().sprite = display;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //지금 드래그하는 창이 상점인지 창고인지
        if (GetComponentInParent<Storage>() != null)
        {
            isShop = false;
        }
        else
        {
            isShop = true;
        }

        //플레이어의 금액 감소 -> 데이타의 itemPrice에서 가져오기
        if (!GameManager.instance.UseMoney(itemData.itemPrice,false) && isShop)
        {
            GameManager.instance.isDragging = false;
            return;
        }
        else
            GameManager.instance.isDragging = true;

        if (!GameManager.instance.isDragging)
            return;

        itemdesc.gameObject.SetActive(false);

        //내가 인벤토리의 어느 부분을 드래그 했는지 가져와야함
        GetOffset(eventData);

        parent = rect.parent;
        siblingIndex = rect.GetSiblingIndex();
        gameObject.transform.SetParent(itemTemp.transform);

        canvasGroup.blocksRaycasts = false;

        //창고에서 인벤토리창이나 상점창으로 옮길때
        if (GetComponentInParent<Storage>())
        {
            transform.parent.gameObject.SetActive(false);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameManager.instance.isDragging)
            return;

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //undercurser를 가져와서 밑에있는 인벤토리창에 적용되는곳을 하이라이트 시켜야함
        //InventorySlot hoverSlot = GetSlotUnderCursor(eventData);

        //if (hoverSlot == null) return;

        //// 원래 좌상단 기준 좌표 보정 (-offset)
        //int placeX = hoverSlot.gridX - offsetX;
        //int placeY = hoverSlot.gridY - offsetY;

        //bool canPlace = inventory.CanPlaceItem(itemData, placeX, placeY);

        //inventory.HighlightSlots(itemData, placeX, placeY, canPlace);
    }

    //샵이나 창고에 있는 아이템을 옮길때 사용되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!GameManager.instance.isDragging)
        {
            return;
        }

        var underCursor = eventData.pointerCurrentRaycast.gameObject;

        //시작이 창고이고 놓았을때 오브젝트가 상점일 경우
        if (underCursor.GetComponent<Shop>() && !isShop)
        {
            //금액의 50퍼센트(내림)반환
            GameManager.instance._money += itemData.itemPrice / 2;
            //이 게임오브젝트를 파괴
            Destroy(gameObject);
            return;
        }
        //놓았을때 오브젝트가 인벤슬롯이 아닐경우
        else if (underCursor.GetComponent<InventorySlot>() == null)
        {
            CantBuyItem();
            return;
        }
        else {  }

        var hoverSlot = underCursor.GetComponentInParent<InventorySlot>();

        //위치시킬 수 있는 곳인지 체크
        if (!inventory.CanPlaceItem(offsetVector, itemData))
        {
            CantBuyItem();
            return;
        }

        //내가들고있는 가방이 아이템일경우와 가방일경우를 나눠서 적용
        if (itemData.itemType == ItemData.ItemType.Bag)
        {
            rect.SetParent(inventory.itemRoot);

            hoverSlot.HasBag = true;
        }
        else
        {
            rect.SetParent(inventory.GetPlaceGrid.transform);
            inventory.OccupySlots(inventory.placeGrid, itemData);
            //상점에서 옮길경우 금액줄고 아닐경우 금액이 줄면 안됨
            //플레이어에게 아이템 효과 적용
            inventory.ApplyItem(itemData, isShop);
        }
        //위치값으로 나오게 조정
        Vector2 pos = inventory.GetSlotWorldPosition(
            inventory.GetPlaceGrid.gridX,
            inventory.GetPlaceGrid.gridY
        );

        rect.anchoredPosition = pos;

        bool isOnSlot = transform.parent.GetComponent<InventorySlot>() != null;
        GetComponent<Image>().sprite = isOnSlot ? panel : display;
        GameManager.instance.isDragging = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemdesc.gameObject.activeSelf)
            itemdesc.gameObject.SetActive(false);
        else
            itemdesc.ShowDesc();
    }

    public void CantBuyItem()
    {
        rect.SetParent(parent);
        rect.SetSiblingIndex(siblingIndex);
        rect.anchoredPosition = Vector2.zero;
        GameManager.instance.isDragging = false;

        canvasGroup.blocksRaycasts = true;
    }
/*CanPlace
    public bool CanPlace(InventorySlot slot)
    {
        //1.현재 내가 둔 슬롯의 위치를 가져옴
        int slotXNum = slot.gridX;
        int slotYNum = slot.gridY;
        int xItemPlaceNum;
        int yItemPlaceNum;
        int itemWidth = itemData.width - 1;
        int itemHeight = itemData.height - 1;
        //2.오프셋 값만큼 빼서 왼쪽 위 슬롯 번호를 가져온다.
        //가로가 왼쪽으로 넘어갔을경우 -> 둔 슬롯의 값을 5로 나눠서 나머지의 값이 offset.x값만큼 뺸값이 -임
        if (slotXNum -offsetVector.x < 0)
        {
            //만약 뺏을때 -값일 경우 x의값은 itemdata의 Width-1값이 되어야한다
            xItemPlaceNum = itemWidth;
        }
        //가로가 오른쪽으로 넘어갔을경우
        else if(slotXNum - offsetVector.x + itemWidth >= 5)
        {
            xItemPlaceNum = 4;
        }
        //인벤토리 안으로 들어갔을경우
        else
        {
            //내가 둔 슬롯 - 오프셋 + 가로크기 -> 맨오른쪽 슬롯
             xItemPlaceNum = slotXNum - offsetVector.x + itemWidth;
        }

        //세로가 위로 넘어갔을때
        if (-slotYNum - offsetVector.y + itemHeight > 0 )
        {
            yItemPlaceNum = itemHeight;
        }
        //세로가 아래로 넘어갔을때
        else if (-slotYNum - offsetVector.y <=-5)
        {
            yItemPlaceNum = 4;
        }
        //인벤토리 안일때
        else
        {
            yItemPlaceNum = slotYNum + offsetVector.y;
        }

        //3.첫 슬롯 번호부터 2중 for문으로 With와 hight만큼의 인벤토리 슬롯에 배치가능 여부를 확인
        for (int i = yItemPlaceNum; i >= yItemPlaceNum - itemHeight; i--)
        {
            for (int j = xItemPlaceNum; j >= xItemPlaceNum - itemWidth; j--)
            {
                if (itemData.itemType == ItemData.ItemType.Bag)
                {
                    if (inventory.grid[j, i].HasBag)
                        return false;
                }
                else
                {
                    //Debug.Log($"grid {j},{i} has item?{inventory.grid[j, i].HasItem}");
                    //Debug.Log($"grid {j},{i} has Bag?{inventory.grid[j, i].HasBag}");
                    if (inventory.grid[j, i].HasItem || !inventory.grid[j, i].HasBag)
                        return false;
                }
            }
        }

        return true;
    }
*/
    public void GetOffset(PointerEventData eventData)
    {
        //piece의 어떤 그리드를 잡았는지 offsetVector를 통하여 알려줌
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePos
        );

        // 0~1 normalized position → 그리드 인덱스 변환
        float normalizedX = (localMousePos.x + rect.rect.width / 2f) / rect.rect.width;
        float normalizedY = (localMousePos.y + rect.rect.height / 2f) / rect.rect.height;

        int offsetX = Mathf.FloorToInt(normalizedX * itemData.width);
        int offsetY = Mathf.FloorToInt(normalizedY * itemData.height);

        offsetVector = new Vector2Int(offsetX, offsetY);
    }
}