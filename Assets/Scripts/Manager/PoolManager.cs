using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //프리펩 보관 변수
    public GameObject[] prefabs;

    // 풀 담당 리스트
    [SerializeField]
    private List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        //비활성화 상태인 게임오브젝트를 찾고 켜줌
        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                Debug.Log("비활성화된 게임오브젝트 찾음");
                break;
            }
        }

        //비활성화되어있는 게임오브젝트가 없으면 새로 생성 후 추가
        if (select == null)
        {
            select = Instantiate(prefabs[index], transform);
            pools[index].Add(select);
            Debug.Log("비활성화된 게임오브젝트 없음 추가함");
        }

        //활성화된 게임오브젝트 반환
        return select;
    }

    //게임오브젝트 활성화된거 비활성화 시켜야됨
    public void Deactive(int index, int num)
    {
        pools[index][num].SetActive(false);
    }
}
