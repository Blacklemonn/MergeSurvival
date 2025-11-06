using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private RectTransform expansionArea;

    //인벤에 위치시킬때 아이템을 가운데로 보정해주는 값
    private Vector2 correctionPos;

    protected InventoryManager inventoryManager;

    protected Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        //inventoryManager
        inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        expansionArea = inventoryManager.slotParent.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //현재 위치값 저장
        originalPosition = rectTransform.anchoredPosition;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //마우스를 따라 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public virtual void Activeslot(Vector2Int gridPos)
    {
    }

    public virtual void Activeslot(Vector2Int gridPos, GameObject item, InventorySlot slot)
    {
    }

    //샵이나 창고에 있는 아이템을 옮길때 사용되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        //마우스의 위치
        Vector2 localPoint;
        //만약 마우스가 인벤토리창 안에 있을경우
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            expansionArea, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            Vector2Int gridPos = ConvertToGrid(localPoint);

            Activeslot(gridPos);

        }
        else { }
    }

    //인벤토리 슬롯에 있는 아이템을 옮길때 사용되는 함수
    public void OnEndDrag(PointerEventData eventData, GameObject item, InventorySlot slot)
    {
        canvasGroup.blocksRaycasts = true;

        //슬롯의 중앙에서 얼마나 떨어진 위치를 드래그 했는지 가져오고 현재 둔 슬롯의 중앙에서 얼마나 떨어진지 위치를 가져와서 두 거리의 차이를 계산
        //if (inventoryManager.prevLocationPos != Vector2.zero)
        //{

        //}
        //마우스의 위치
        Vector2 localPoint;
        //만약 마우스가 인벤토리창 안에 있을경우
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            expansionArea, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            Vector2Int gridPos = ConvertToGrid(localPoint);

            Activeslot(gridPos, item, slot);
        }
        else { }
    }

    //인벤토리 패널 그리드의 몇번째줄 몇칸인지 알려줌
    public Vector2Int ConvertToGrid(Vector2 localPoint)
    {
        GridLayoutGroup grid = expansionArea.GetComponent<GridLayoutGroup>();

        float cellWidth = grid.cellSize.x;
        float cellHeight = grid.cellSize.y;

        float spacingX = grid.spacing.x;
        float spacingY = grid.spacing.y;

        int constraintCount = grid.constraintCount;
        int ChildCount = grid.GetComponent<RectTransform>().transform.childCount;

        Vector2 tempPos;

        if (grid.childAlignment == TextAnchor.UpperLeft)
        {
            Debug.Log(grid.childAlignment);
            float offsetX = expansionArea.rect.width * expansionArea.pivot.x + (grid.padding.left);
            float offsetY = expansionArea.rect.height * expansionArea.pivot.y - (grid.padding.top);

            float relativeX = localPoint.x - offsetX;
            float relativeY = -(localPoint.y - offsetY); // y는 위에서 아래로 내려가니까 부호 반전

            tempPos = new Vector2(relativeX % (cellWidth + spacingX), relativeY % (cellHeight + spacingY));
            //뒀을때 가운데 기준으로 얼만큼 떨어져 있는지 확인하고
            //아이템의 위치를 가운데로 보정시켜줘야함
            correctionPos = new Vector2(cellWidth/2, cellHeight/2) - tempPos;

                int x = Mathf.FloorToInt(relativeX / (cellWidth + spacingX));
            int y = Mathf.FloorToInt(relativeY / (cellHeight + spacingY));

            return new Vector2Int(x, y);
        }
        else if (grid.childAlignment == TextAnchor.MiddleCenter)
        {
            Debug.Log(grid.childAlignment);

            //constcount의 값을 2로 나누어 수평 오프셋값 설정
            //constcount의 값으로 자식값을 나누고 +1 한 값을 2로 나누어 수직 오프셋값 설정
            //2를 먼저 나누면 소수점이 사라지니까 크기를 먼저 곱할것
            float offsetX = -(constraintCount * cellWidth / 2) + (grid.padding.left);
            float offsetY = ((ChildCount / constraintCount) * cellHeight / 2) - (grid.padding.top);

            float relativeX = localPoint.x - offsetX;
            float relativeY = -(localPoint.y - offsetY); // y는 위에서 아래로 내려가니까 부호 반전

            tempPos = new Vector2(relativeX % (cellWidth + spacingX), relativeY % (cellHeight + spacingY));
            //뒀을때 가운데 기준으로 얼만큼 떨어져 있는지 확인하고
            //아이템의 위치를 가운데로 보정시켜줘야함
            correctionPos = new Vector2(cellWidth / 2, cellHeight / 2) - tempPos;

            int x = Mathf.FloorToInt(relativeX / (cellWidth + spacingX));
            int y = Mathf.FloorToInt(relativeY / (cellHeight + spacingY));


            return new Vector2Int(x, y);
        }
        else
        {
            Debug.Log($"only support MiddleCenter, UpperLeft \n current aligment : {grid.childAlignment}");
            return new Vector2Int(0, 0);
        }
    }
}
