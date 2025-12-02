using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemData itemData;

    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private RectTransform rect;
    private Transform parent;
    private int siblingIndex;
    private Sprite display;
    private Sprite panel;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }
    private void Start()
    {
        if(itemData != null)
        ChangeItemData(itemData);
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
        parent = rect.parent;
        siblingIndex = rect.GetSiblingIndex();

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    //샵이나 창고에 있는 아이템을 옮길때 사용되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        var underCursor = eventData.pointerCurrentRaycast.gameObject;
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
                rect.SetParent(parent);
                rect.SetSiblingIndex(siblingIndex);
                canvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            if (slot.gameObject.transform.childCount == 1)
            {
                rect.SetParent(underCursor.transform);
                //플레이어에게 아이템 효과 적용

            }
            else
            {
                rect.SetParent(parent);
                rect.SetSiblingIndex(siblingIndex);
                canvasGroup.blocksRaycasts = true;
            }
        }

        rect.anchoredPosition = Vector2.zero;

        bool isOnSlot = transform.parent.GetComponent<InventorySlot>() != null;
        //Debug.Log(isOnSlot);
        GetComponent<Image>().sprite = isOnSlot ? panel : display;
    }
}