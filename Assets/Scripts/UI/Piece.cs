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

    private bool isShop;

    [SerializeField]
    private ItemDesc itemdesc;

    private Inventory inventory;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        itemTemp = GameManager.instance.itemTemp;
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
        panel = itemData.itemIcon;
        display = itemData.displayIcon;
        GetComponent<Image>().sprite = display;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //플레이어의 금액 감소 -> 데이타의 itemPrice에서 가져오기
        if (!GameManager.instance.UseMoney(itemData.itemPrice,false))
        {
            GameManager.instance.isDragging = false;
            return;
        }
        else
            GameManager.instance.isDragging = true;

        if (!GameManager.instance.isDragging)
            return;

        //지금 드래그하는 창이 상점인지 창고인지
        if (GetComponentInParent<Storage>() != null)
        {
            isShop = false;
        }
        else
        {
            isShop = true;
        }

        itemdesc.gameObject.SetActive(false);

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
            Debug.Log("창고 -> 상점");
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

        var slot = underCursor.GetComponent<InventorySlot>();

        //내가들고있는 가방이 아이템일경우와 가방일경우를 나눠서 적용
        if (itemData.itemType == ItemData.ItemType.Bag)
        {
            if (slot.gameObject.transform.childCount == 0)
            {
                rect.SetParent(underCursor.transform);
            }
            else
            {
                CantBuyItem();
            }
        }
        else
        {
            if (slot.gameObject.transform.childCount == 1)
            {
                rect.SetParent(underCursor.transform);
                //상점에서 옮길경우 금액줄고 아닐경우 금액이 줄면 안됨
                //플레이어에게 아이템 효과 적용
                inventory.ApplyItem(itemData, isShop);
            }
            else
            {
                CantBuyItem();
            }
        }

        rect.anchoredPosition = Vector2.zero;

        bool isOnSlot = transform.parent.GetComponent<InventorySlot>() != null;
        //Debug.Log(isOnSlot);
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
}