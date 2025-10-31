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

    public void SetItemParent(GameObject item)
    {

        // 인벤토리 창에 있는경우 (구매를 한 상태)
        // 둘 수 없는 위치에 뒀을 경우 -> 기존의 위치로 돌아감(뺄때의 상태를 저장)

        // 인벤토리 밖에 뒀을 경우 -> 창고로 들어감
        rectTransform.anchoredPosition = originalPosition;
    }

    public virtual void ReturnToOriginalSlot(Vector2Int gridPos, GameObject item, InventorySlot slot)
    {
        //아이템의 기존 저장값을 가져와서 되돌려야함 -> 드래그 비긴을 했을때 자신의 상태를 pop하니까 다시 stack을 하게 해야함
        //스택은 인벤토리 슬롯에 있음 인벤토리 슬롯의 스택에 다시 넣어야함
    }


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

    public void OnEndDrag(PointerEventData eventData, GameObject item, InventorySlot slot)
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
        else
        {
            Vector2Int gridPos = ConvertToGrid(localPoint);

            ReturnToOriginalSlot(gridPos, item, slot);
        }
    }

    private Vector2Int ConvertToGrid(Vector2 localPoint)
    {
        GridLayoutGroup grid = expansionArea.GetComponent<GridLayoutGroup>();

        float cellWidth = grid.cellSize.x;
        float cellHeight = grid.cellSize.y;

        float spacingX = grid.spacing.x;
        float spacingY = grid.spacing.y;

        int constraintCount = grid.constraintCount;
        int ChildCount = grid.GetComponent<RectTransform>().transform.childCount;

        if (grid.childAlignment == TextAnchor.UpperLeft)
        {
            Debug.Log(grid.childAlignment);
            float offsetX = -expansionArea.rect.width * expansionArea.pivot.x + (grid.padding.left);
            float offsetY = expansionArea.rect.height * expansionArea.pivot.y - (grid.padding.top);

            float relativeX = localPoint.x - offsetX;
            float relativeY = -(localPoint.y - offsetY); // y는 위에서 아래로 내려가니까 부호 반전

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

            int x = Mathf.FloorToInt(relativeX / (cellWidth + spacingX));
            int y = Mathf.FloorToInt(relativeY / (cellHeight + spacingY));

            return new Vector2Int(x, y);
        }
        else
        {
            Debug.Log(grid.childAlignment);
            return new Vector2Int(0, 0);
        }
    }
}
