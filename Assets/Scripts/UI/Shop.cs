using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Shop : MonoBehaviour
{
    //하위아이템으로 생성될 아이템
    [SerializeField]
    private GameObject[] shopGoods;
    [SerializeField]
    private ItemData[] itemDatas;
    [SerializeField]
    private GameObject prefab;

    public GameObject storage;
    public GameObject shop;

    private void Update()
    {
        //상점창과 창고창을 스위치
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (!this.gameObject.activeSelf)
                return;

            storage.SetActive(!storage.activeSelf);
            shop.SetActive(!shop.activeSelf);

            if (!storage.activeSelf)
                GameManager.instance.storage.BaseArray();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!shop.activeSelf)
                return;
            ReRoll();
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        ReRoll();
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

    //4개의 게임 오브젝트 하위로 아이템 랜덤생성
    public void ReRoll()
    {
        //들고있는 아이템이 사라져야함
        if (GameManager.instance.isDragging)
            return;
        //클릭된 설명창이 사라져야함
        
        //상점 슬롯 횟수만큼 반복
        for (int i = 0; i < shopGoods.Length; i++)
        {
            int rand = Random.Range(0, itemDatas.Length);
            Piece goodPiece = null;
            if(shopGoods[i].GetComponentInChildren<Piece>())
                goodPiece = shopGoods[i].GetComponentInChildren<Piece>();

            //자식으로 게임 오브젝트가 있는지?(piece스크립트
            if (goodPiece == null)
            {
                GameObject piece = Instantiate(prefab, shopGoods[i].transform);
                piece.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                //없을경우 -> 오브젝트 생성후 피스의 아이템 데이타 랜덤 돌린거 넣어주기
                goodPiece = piece.GetComponentInChildren<Piece>();
                goodPiece.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                goodPiece = shopGoods[i].GetComponentInChildren<Piece>();
            }

            if(!goodPiece.gameObject.activeSelf)
                goodPiece.gameObject.SetActive(true);

            goodPiece.ChangeItemData(itemDatas[rand]);
        }
    }

    public void Select(int CharNum)
    {
        GameManager.instance.inventory.ApplyItem(itemDatas[CharNum], false);
    }
}