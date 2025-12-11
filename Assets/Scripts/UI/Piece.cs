using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

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

    [SerializeField]
    private ItemDesc itemdesc;

    private Shop shop;

    void Awake()
    {
        shop = GetComponentInParent<Shop>();
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        itemTemp = GameManager.instance.itemTemp;
    }
    private void Start()
    {
        if (itemData != null)
            ChangeItemData(itemData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rect.anchoredPosition = Vector2.zero;
            Debug.Log(rect.anchoredPosition);
        }
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
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //undercurser를 가져와서 밑에있는 인벤토리창에 적용되는곳을 하이라이트 시켜야함
    }

    //샵이나 창고에 있는 아이템을 옮길때 사용되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        //플레이어가 가지고 있는돈이 아이템의 금액보다 적은지 아닌지
        //플레이어의 금액 감소 -> 데이타의 itemPrice에서 가져오기
        if (GameManager.instance.UseMoney(itemData.itemPrice))
        {
            GameManager.instance.UseMoney(itemData.itemPrice);
        }
        else
        {
            CantBuyItem();
            return;
        }

        var underCursor = eventData.pointerCurrentRaycast.gameObject;

        if (underCursor.GetComponent<Storage>())
        {
            GameManager.instance.storage.GotoStorage(this.gameObject);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            canvasGroup.blocksRaycasts = true;
            return;
        }

        //언더커서의 오브젝트가 인벤슬롯일경우만 실행
        if (underCursor.GetComponent<InventorySlot>() == null)
        {
            CantBuyItem();
            return;
        }

        var slot = underCursor.GetComponent<InventorySlot>();

        //Debug.Log(slot == null ? "null" : slot.name);


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
                //플레이어에게 아이템 효과 적용
                shop.ApplyItem(itemData);
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

        canvasGroup.blocksRaycasts = true;
    }
}