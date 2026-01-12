using UnityEngine;
using UnityEngine.EventSystems;
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
    rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

    var obj = eventData.pointerCurrentRaycast.gameObject;
    InventorySlot slot = obj?.GetComponentInParent<InventorySlot>();

    if (slot == null)
        {
            inventory.ClearHighlight();
            return;
        }


    Vector2Int placePos;

    if (!inventory.TryGetPlacePosition(slot, offsetVector, itemData, out placePos))
    {
        inventory.ClearHighlight();
        return;
    }

    bool canPlace = inventory.CanPlaceItem(placePos, itemData);

    inventory.Highlight(placePos, itemData, canPlace);
}

    //샵이나 창고에 있는 아이템을 옮길때 사용되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        inventory.ClearHighlight();

        if (!GameManager.instance.isDragging)
            return;

        GameObject obj = eventData.pointerCurrentRaycast.gameObject;
        InventorySlot slot = obj?.GetComponentInParent<InventorySlot>();

        if (slot == null)
        {
            CantBuyItem();
            return;
        }

        Vector2Int placePos;
        //둘수 없는곳에 있을때
        if (!inventory.TryGetPlacePosition(slot, offsetVector, itemData, out placePos))
        {
            CantBuyItem();
            return;
        }

        //조건이 맞지 않을때
        if (!inventory.CanPlaceItem(placePos, itemData))
        {
            CantBuyItem();
            return;
        }

        // === 여기부터 성공 ===

        rect.SetParent(inventory.itemRoot);

        Vector2 uiPos = inventory.GetSlotWorldPosition(placePos.x, placePos.y);
        rect.anchoredPosition = uiPos;

        //가방일 경우 슬롯의 스프라이트를 바꿀까?
        GetComponent<Image>().sprite = panel;

        //슬롯의 상태를 변경해줘야됨
        //inventory.CanPlaceItem(placePos, itemData);

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

    public void GetOffset(PointerEventData eventData)
    {
        Vector2 localMousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePos
        );

        Rect r = rect.rect;

        float normalizedX = (localMousePos.x - r.xMin) / r.width;
        float normalizedY = (localMousePos.y - r.yMin) / r.height;

        // 0~1 clamp
        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedY = Mathf.Clamp01(normalizedY);

        int offsetX = Mathf.FloorToInt(normalizedX * itemData.width);
        int offsetY = Mathf.FloorToInt(normalizedY * itemData.height);

        // 인덱스 안전 보정
        offsetX = Mathf.Clamp(offsetX, 0, itemData.width - 1);
        offsetY = Mathf.Clamp(offsetY, 0, itemData.height - 1);
        offsetY = itemData.height - 1 - offsetY;

        offsetVector = new Vector2Int(offsetX, offsetY);
    }
}