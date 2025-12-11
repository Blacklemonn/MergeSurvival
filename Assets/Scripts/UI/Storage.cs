using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField]
    private GameObject[] slots;
    //아이템 인자를 받아오면 그 아이템을 오브젝트의 자식으로 이동
    public void GotoStorage(GameObject Item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].activeSelf && slots[i].transform.childCount == 0)
            {
                Item.transform.SetParent(slots[i].transform);
                return;
            }
            else if (!slots[i].activeSelf)
            {
                slots[i].SetActive(true);
                Item.transform.SetParent(slots[i].transform);
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