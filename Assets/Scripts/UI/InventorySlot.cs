using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform itemRect;
    private Canvas canvas;

    [SerializeField]
    private GameObject itemTemp;

    private bool isItem;
    private Transform itemObj;
    private ItemData data;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        itemTemp = GameManager.instance.itemTemp;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.childCount == 0)
            return;

        isItem = transform.childCount == 2 ? true : false;

        if (itemObj = transform.GetChild(transform.childCount - 1) as Transform)
        {
            itemRect = itemObj.GetComponent<RectTransform>();
            itemObj.SetParent(itemTemp.transform);
            data = itemObj.GetComponent<Piece>().itemData;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemRect == null)
            return;

        itemRect.anchoredPosition += eventData.delta / canvas.scaleFactor;

        //undercurser를 가져와서 밑에있는 인벤토리창에 적용되는곳을 하이라이트 시켜야함
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemRect == null)
            return;

        var underCursor = eventData.pointerCurrentRaycast.gameObject;

        //언더커서의 오브젝트가 창고창일때 실행
        if (underCursor.GetComponent<Storage>())
        {
            GameManager.instance.storage.GotoStorage(itemObj.gameObject);

            //아이템 효과 제거
            GameManager.instance.inventory.RemoveItem(data);
            itemRect.offsetMin = Vector2.zero;
            itemRect.offsetMax = Vector2.zero;
            itemObj.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            return;
        }
        //언더커서의 오브젝트가 상점일 경우
        else if (underCursor.GetComponent<Shop>())
        {
            //금액의 50퍼센트(내림)반환
            GameManager.instance._money += data.itemPrice / 2;
            //아이템 효과 제거
            GameManager.instance.inventory.RemoveItem(data);
            //이 게임오브젝트를 파괴
            Destroy(itemObj.gameObject);
            return;
        }
        else if (!underCursor.GetComponent<InventorySlot>())
        {
            itemRect.transform.SetParent(this.transform);
            itemRect.anchoredPosition = Vector3.zero;
            data = null;
            return;
        }
        else { }

        if (isItem)
        {
            //들고있는 오브젝트가 아이템
            //자식이 1개있어야 적용됨
            if (underCursor.transform.childCount == 1)
            {
                itemRect.transform.SetParent(underCursor.transform);
            }
            else
                itemRect.transform.SetParent(this.transform);
        }
        else
        {
            //들고있는 오브젝트가 가방
            //자식이 0개있어야 적용됨
            if (underCursor.transform.childCount == 0)
                itemRect.transform.SetParent(underCursor.transform);
            else
                itemRect.transform.SetParent(this.transform);
        }
        itemRect.anchoredPosition = Vector3.zero;
        itemRect = null;
        data = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.childCount == 2)
        {
            transform.GetChild(1).GetComponent<Piece>().OnPointerClick(eventData);
        }
        else { }
    }
}