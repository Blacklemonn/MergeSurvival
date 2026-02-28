using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ItemBelongState
{
    Shop,
    Storage,
    Inventory
}

public class Piece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public const float SLOT_SIZE = 12f;

    public ItemData itemData;
    public ItemBelongState state;

    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private RectTransform rect;
    private Transform parent;
    private GameObject itemTemp;
    private Vector2 prevAnchorPos;

    [HideInInspector]
    public bool canMerge = true;

    public List<InventorySlot> arrangeSlot;

    [SerializeField]
    private ItemDesc itemdesc;

    private InventoryManager inventory;
    //내가 잡은 아이템 위치
    private Vector2Int offsetVector;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        itemTemp = GameManager.instance.itemTemp;
        arrangeSlot = new List<InventorySlot>();
        inventory = GameManager.instance.inventory;
        state = ItemBelongState.Shop;
    }

    private void Start()
    {
        if (itemData != null)
            ChangeItemData(itemData);
    }

    public void ChangeItemData(ItemData data)
    {
        itemData = data;
        //크기에 맞게 리사이징
        var size = new Vector2(itemData.width * SLOT_SIZE, itemData.height * SLOT_SIZE);
        rect.sizeDelta = size;
        GetComponent<Image>().sprite = itemData.itemIcon;

        itemdesc.ChangeDes(data);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //플레이어의 자금으로 살수 있는지 -> 데이타의 itemPrice에서 가져오기
        if (!GameManager.instance.UseMoney(itemData.itemPrice, false) && state == ItemBelongState.Shop)
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

        //슬롯에서 꺼낼때 아이템을 가지고있던 슬롯의 상태를 수정해야함
        foreach (InventorySlot sl in arrangeSlot)
        {
            if (itemData.itemType == ItemData.ItemType.Bag)
            {
                sl.HasBag = false;
                sl.GetComponent<Image>().sprite = inventory.slotSprite[0];
            }
            else
            {
                sl.HasItem = false;
            }
        }

        //내가 들었을때 인벤토리에 조합목록에 이 아이템이 들어있을경우 없애줘야함
        inventory.RemoveMergeList(this);

        parent = rect.parent;
        prevAnchorPos = rect.anchoredPosition;
        gameObject.transform.SetParent(itemTemp.transform);

        inventory.HighlightItem(itemData);

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameManager.instance.isDragging)
            return;

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

        var obj = eventData.pointerCurrentRaycast.gameObject;
        InventorySlot slot = obj?.GetComponentInParent<InventorySlot>();

        if (slot == null)
        {
            inventory.ClearHighlightSlot();
            return;
        }


        Vector2Int placePos;

        if (!inventory.TryGetPlacePosition(slot, offsetVector, itemData, out placePos))
        {
            inventory.ClearHighlightSlot();
            return;
        }

        bool canPlace = inventory.CanPlaceItem(placePos, itemData);

        inventory.HighlightSlot(placePos, itemData, canPlace);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventory.ClearHighlightSlot();

        if (!GameManager.instance.isDragging)
            return;

        GameObject underCurserObj = eventData.pointerCurrentRaycast.gameObject;

        //창고에 뒀을때
        if (underCurserObj.GetComponent<Storage>())
        {
            switch (state)
            {
                case ItemBelongState.Storage:
                    CantBuyItem();
                    return;
                case ItemBelongState.Inventory:
                    underCurserObj.GetComponent<Storage>().GotoStorage(this.gameObject);

                    if (itemData.itemType == ItemData.ItemType.Bag)
                    {
                        foreach (InventorySlot sl in arrangeSlot)
                        {
                            sl.bagObj = null;
                        }
                        arrangeSlot.Clear();
                    }
                    else
                    {
                        foreach (InventorySlot sl in arrangeSlot)
                        {
                            sl.itemObj = null;
                        }
                        arrangeSlot.Clear();
                        inventory.RemoveItem(itemData);
                    }

                    Init();
                    return;
                case ItemBelongState.Shop:
                    underCurserObj.GetComponent<Storage>().GotoStorage(this.gameObject);

                    GameManager.instance._money -= itemData.itemPrice;
                    Init();
                    return;
            }
        }
        //상점에 뒀을때
        else if (underCurserObj.tag == "Shop")
        {
            switch (state)
            {
                case ItemBelongState.Shop:
                    CantBuyItem();
                    return;
                case ItemBelongState.Inventory:
                    inventory.RemoveItem(itemData);
                    break;
                case ItemBelongState.Storage:
                    break;
            }

            GameManager.instance._money -= Mathf.FloorToInt(itemData.itemPrice / 2);
            Init();
            Destroy(this.gameObject);
            return;
        }



        InventorySlot slot = underCurserObj?.GetComponentInParent<InventorySlot>();

        //슬롯이 아닌곳에 둘때
        if (slot == null)
        {
            CantBuyItem();
            return;
        }

        Vector2Int placePos;
        //아이템이 외곽으로 튀어나올때
        if (!inventory.TryGetPlacePosition(slot, offsetVector, itemData, out placePos))
        {
            CantBuyItem();
            return;
        }

        //조건이 맞지 않을때
        if (!inventory.CanPlaceItem(placePos, itemData))
        {
            //가방일때
            if (itemData.itemType == ItemData.ItemType.Bag)
            {
                foreach (InventorySlot sl in arrangeSlot)
                {
                    //이전에 있던 슬롯 다시 가방상태로 변경 후 가방 안보이게
                    sl.GetComponent<Image>().sprite = inventory.slotSprite[1];
                    sl.HasBag = true;
                    this.gameObject.SetActive(false);
                    //원위치로

                    //아이템을 슬롯의 위치로 이동
                    rect.anchoredPosition = inventory.grid[sl.gridX, sl.gridY].GetComponent<RectTransform>().anchoredPosition;

                    //아이템의 위치를 보정
                    rect.anchoredPosition += new Vector2(itemData.width == 1 ? 0 : (SLOT_SIZE / 2) * (itemData.width - 1), itemData.height == 1 ? 0 : -(SLOT_SIZE / 2) * (itemData.height - 1));

                    GameManager.instance.isDragging = false;

                    canvasGroup.blocksRaycasts = true;
                }
            }
            else
                CantBuyItem();
            return;
        }


        //------------------------------------------------------------------------------
        //                          여기부터 슬롯에 두기 성공
        //------------------------------------------------------------------------------

        if (itemData.itemType == ItemData.ItemType.Bag)
        {
            //이전 slot의 데이터를 지워줌
            foreach (InventorySlot sl in arrangeSlot)
            {
                sl.bagObj = null;
            }

            arrangeSlot.Clear();
        }
        else
        {
            //이전 slot의 데이터를 지워줌
            foreach (InventorySlot sl in arrangeSlot)
            {
                sl.itemObj = null;
            }

            arrangeSlot.Clear();
        }

        //itemRoot로 이동
        rect.SetParent(inventory.itemRoot);

        //슬롯의 상태를 변경해줘야됨 , 현재 이 아이템이 적용되고 있는 슬롯 저장
        inventory.PlaceItem(placePos, itemData, out arrangeSlot);

        //아이템이 가방일 경우
        if (itemData.itemType == ItemData.ItemType.Bag)
        {
            //slot의 bagObj에 이 오브젝트를 저장해줘야함
            foreach (InventorySlot sl in arrangeSlot)
            {
                sl.bagObj = this;
            }
            //가방의 오브젝트를 꺼줘야됨
            gameObject.SetActive(false);
        }
        else
        {
            //slot의 itemObj에 이 오브젝트를 저장해줘야함
            foreach (InventorySlot sl in arrangeSlot)
            {
                sl.itemObj = this;
            }



            //인벤토리상태가 아닐경우 캐릭터에 적용
            if (state != ItemBelongState.Inventory)
                inventory.ApplyItem(itemData, state == ItemBelongState.Shop ? true : false);

            //주변에 합칠 수 있는게 있는지 확인
            TryItemMerge();
        }

        //아이템을 슬롯의 위치로 이동
        rect.anchoredPosition = inventory.grid[placePos.x, placePos.y].GetComponent<RectTransform>().anchoredPosition;

        //아이템의 위치를 보정
        rect.anchoredPosition += new Vector2(itemData.width == 1 ? 0 : (SLOT_SIZE / 2) * (itemData.width - 1), itemData.height == 1 ? 0 : -(SLOT_SIZE / 2) * (itemData.height - 1));

        state = ItemBelongState.Inventory;

        Init();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        itemdesc.gameObject.SetActive(!itemdesc.gameObject.activeSelf);
    }

    private void GetOffset(PointerEventData eventData)
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

    public void CantBuyItem()
    {
        rect.SetParent(parent);
        rect.anchoredPosition = prevAnchorPos;
        inventory.ClearHighlightItem();
        GameManager.instance.isDragging = false;

        canvasGroup.blocksRaycasts = true;
    }

    private void Init()
    {
        //다시 아이템을 집을 수 있게 수정 -> 문제발생
        canvasGroup.blocksRaycasts = true;

        inventory.ClearHighlightItem();

        GameManager.instance.isDragging = false;
    }

    public void TryItemMerge() // Second 매개변수 제거
    {
        List<InventorySlot> nearSlots = new List<InventorySlot>();
        Dictionary<ItemData, List<Piece>> nearPieces = new Dictionary<ItemData, List<Piece>>();

        // 내 주변 슬롯 수집
        foreach (InventorySlot slot in arrangeSlot)
        {
            slot.NearSlots(nearSlots);
        }

        // 나 자신을 먼저 포함
        nearPieces[itemData] = new List<Piece> { this };

        // 주변 아이템 수집
        foreach (InventorySlot slot in nearSlots)
        {
            if (slot.itemObj != null && slot.itemObj != this && slot.itemObj.canMerge)
            {
                ItemData data = slot.itemObj.itemData;
                if (!nearPieces.ContainsKey(data)) nearPieces[data] = new List<Piece>();

                if (!nearPieces[data].Contains(slot.itemObj))
                {
                    nearPieces[data].Add(slot.itemObj);
                }
            }
        }

        inventory.MergeItem(nearPieces);
    }

    public void ClearSlotsItem()
    {
        foreach (InventorySlot sl in arrangeSlot)
        {
            sl.itemObj = null;
            sl.HasItem = false;
        }

        arrangeSlot.Clear();
    }

    public void MoveTo(Vector2 pos)
    {
        StartCoroutine(MoveRectEaseOut(pos));
    }

    IEnumerator MoveRectEaseOut(Vector2 pos)
    {
        float elapsdTime = 0f;
        float duration = 1f;
        Vector2 startPos = rect.position;

        while (elapsdTime < duration)
        {
            elapsdTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsdTime / duration);

            float easedT = 1f - Mathf.Pow(1f - t, 3);

            rect.position = Vector2.Lerp(startPos, pos, easedT);

            yield return null;
        }

        rect.anchoredPosition = pos;
    }
}