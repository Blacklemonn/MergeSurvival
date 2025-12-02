using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform item;
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
            item = itemObj.GetComponent<RectTransform>();
            itemObj.SetParent(itemTemp.transform);
            //item.gameObject.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
            return;
        
        item.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        var underCursor = eventData.pointerCurrentRaycast.gameObject;

        if (!underCursor.GetComponent<InventorySlot>())
        {
            item.transform.SetParent(this.transform);
            item.anchoredPosition = Vector3.zero;
            return;
        }

        //Debug.Log(underCursor.name);

        if (isItem)
        {
            //창고 창으로 뒀을때 아이템 효과 제거


            //들고있는 오브젝트가 아이템
            //자식이 1개있어야 적용됨
            if (underCursor.transform.childCount == 1)
            {
                item.transform.SetParent(underCursor.transform);
            }
            else
                item.transform.SetParent(this.transform);
        }
        else
        {
            //들고있는 오브젝트가 가방
            //자식이 0개있어야 적용됨
            if (underCursor.transform.childCount == 0)
                item.transform.SetParent(underCursor.transform);
            else
                item.transform.SetParent(this.transform);
        }
        item.anchoredPosition = Vector3.zero;
        item = null;
    }
}