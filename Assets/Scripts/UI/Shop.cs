using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public Item item;

    //하위아이템으로 생성될 아이템
    [SerializeField]
    private GameObject[] shopGoods;

    [SerializeField]
    private ItemData[] itemDatas;

    [SerializeField]
    private GameObject prefab;

    public void Show()
    {
        gameObject.SetActive(true);
        GameManager.instance.Stop();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.EffectBGM(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.EffectBGM(false);
    }

    public void Select(int index)
    {
        item.OnClick();
    }

    //4개의 게임 오브젝트 하위로 아이템 랜덤생성
    public void ReRoll()
    {
        //상점 슬롯 횟수만큼 반복
        for (int i = 0; i < shopGoods.Length; i++)
        {
            int rand = Random.Range(0, itemDatas.Length);

            Piece goodsPiece = shopGoods[i].GetComponentInChildren<Piece>();
            //자식으로 게임 오브젝트가 있는지?(piece스크립트
            if (shopGoods[i].transform.childCount == 0)
            {
                //없을경우 -> 오브젝트 생성후 피스의 아이템 데이타 랜덤 돌린거 넣어주기
                Instantiate(prefab, shopGoods[i].transform);
            }
            else { }

            //Debug.Log(goodsPiece.gameObject.name);

            goodsPiece.ChangeItemData(itemDatas[rand]);
        }
    }

}
