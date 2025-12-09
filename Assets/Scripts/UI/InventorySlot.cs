using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private RectTransform itemRect;
    private Canvas canvas;

    [SerializeField]
    private GameObject itemTemp;

    private bool isItem;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        itemTemp = GameObject.Find("ItemTemp");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (transform.childCount == 0)
            return;

        Transform itemObj;

        isItem = transform.childCount == 2 ? true : false;

        if (itemObj = transform.GetChild(transform.childCount - 1) as Transform)
        {
            itemRect = itemObj.GetComponent<RectTransform>();
            itemObj.SetParent(itemTemp.transform);
            //item.gameObject.SetActive(true);
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

        if (!underCursor.GetComponent<InventorySlot>())
        {
            itemRect.transform.SetParent(this.transform);
            itemRect.anchoredPosition = Vector3.zero;
            return;
        }

        //언더커서의 오브젝트가 창고창일때 실행

        //언더커서의 오브젝트가 상점창일때 실행

        //Debug.Log(underCursor.name);

        if (isItem)
        {
            //창고 창으로 뒀을때 아이템 효과 제거

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