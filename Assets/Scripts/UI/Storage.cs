using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField]
    private GameObject[] slots;
    //아이템 인자를 받아오면 그 아이템을 오브젝트의 자식으로 이동
    public void GotoStorage(GameObject item)
    {
        item.GetComponent<Piece>().state = ItemBelongState.Storage;

        ItemData data = item.GetComponent<Piece>().itemData;

        Vector2 lerpPos = new Vector2(data.width == 1 ? Piece.SLOT_SIZE/2 : Piece.SLOT_SIZE * data.width / 4, data.height == 1 ? -Piece.SLOT_SIZE/2 : -Piece.SLOT_SIZE * data.height / 4);
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].activeSelf && slots[i].transform.childCount == 0)
            {
                item.transform.SetParent(slots[i].transform);
                item.GetComponent<RectTransform>().anchoredPosition = lerpPos;
                return;
            }
            else if (!slots[i].activeSelf)
            {
                slots[i].SetActive(true);
                item.transform.SetParent(slots[i].transform);
                item.GetComponent<RectTransform>().anchoredPosition = lerpPos;
                return;
            }
            else { }
        }
    }

    public void BaseArray()
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            if (slots[i].activeSelf && slots[i].transform.childCount < 1)
            {
                slots[i].SetActive(false);
            }
        }
    }
}