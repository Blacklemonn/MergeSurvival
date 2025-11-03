using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler ,IDragHandler, IEndDragHandler
{
    //슬롯의 아이템이 활성화 상태인가
    public bool isOccupied = false;
    //슬롯의 가방이 활성화 상태인가
    [HideInInspector]
    public bool isActive = false;
    public Image itemIcon;
    public Vector2Int gridPosition;

    public Sprite UnLockSlotImage;
    public Sprite LockSlotImage;

    private Stack<GameObject> slotItemList = new();
    private GameObject itemObj;
    private RectTransform rectTransform;
    public Vector2 originAnchorPos;

    private Canvas canvas;
    private bool CanDrag;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    private void Start()
    {
        ClearItem();
        ClearInventory();
    }

    public void stackItem(GameObject item)
    {
        Debug.Log(item);
        slotItemList.Push(item);
        item.SetActive(false);
    }
    public GameObject popItem()
    {
        if(slotItemList.Count <= 0)
            return null;

        itemObj = slotItemList.Pop();
        itemObj.SetActive(true);

        return itemObj;
    }

    public void ActiveInventory()
    {
        isActive = true;
        GetComponent<Image>().sprite = UnLockSlotImage;
    }

    public void ClearInventory()
    {
        isActive = false;
        GetComponent<Image>().sprite = LockSlotImage;
    }

    public void AddItem(Sprite icon)
    {
        isOccupied = true;
        itemIcon.sprite = icon;
        itemIcon.enabled = true;
        GetComponentInChildren<Image>().sprite = icon;
    }

    public void ClearItem()
    {
        isOccupied = false;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        GetComponentInChildren<Image>().sprite = UnLockSlotImage;
    }

    //어떤 슬롯을 클릭했는지 시각화 or 나중에 아이템 설명 UI를 띄우기
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //dragStart

        Debug.Log("OnBeginDrag");

        //클릭된 슬롯창에 아이템이 있는지 없는지 확인
        if (isOccupied)
        {
            //없을경우 현재 위치에 슬롯에 맞는 가방 생성
            rectTransform = popItem().GetComponent<RectTransform>();
            originAnchorPos = rectTransform.anchoredPosition;

            itemObj.transform.parent = inventoryManager.tempItemStorage.transform;

            //슬롯의 아이템 빈칸으로 변경
            ClearItem();

            CanDrag = true;
        }
        else
        {
            //가방이 비활성화된 상태일 경우 아무일도 일어나지 않음
            if (!isActive)
                return;


            //없을경우 현재 위치에 슬롯에 맞는 가방 생성
            rectTransform = popItem().GetComponent<RectTransform>();
            originAnchorPos = rectTransform.anchoredPosition;

            itemObj.transform.parent = inventoryManager.tempItemStorage.transform;

            //슬롯의 가방 크기에 맞게 비활성화
            ClearInventory();
            
            CanDrag = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!CanDrag)
            return;

        //마우스를 따라 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (!CanDrag)
            return;

        //가방일경우
        if (slotItemList.Count == 0)
        {
            itemObj.GetComponent<BagPieceUI>().OnEndDrag(eventData, itemObj, this.GetComponent<InventorySlot>());
        }
        //아이템일경우
        else if (slotItemList.Count == 1)
        {
            itemObj.GetComponent<EquipmentUI>().OnEndDrag(eventData, itemObj, this.GetComponent<InventorySlot>());
        }
        else { }



        CanDrag = false;
    }
}