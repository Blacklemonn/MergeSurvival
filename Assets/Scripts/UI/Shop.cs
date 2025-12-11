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

    private Weapon weapon;
    private Gear gear;

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

    public void ItemInit()
    {
        foreach (var item in itemDatas)
        {
            item.itemQuantity = 0;
        }
    }

    //4개의 게임 오브젝트 하위로 아이템 랜덤생성
    public void ReRoll()
    {
        //상점 슬롯 횟수만큼 반복
        for (int i = 0; i < shopGoods.Length; i++)
        {
            int rand = Random.Range(0, itemDatas.Length);

            Piece goodsPiece;

            //자식으로 게임 오브젝트가 있는지?(piece스크립트
            if (shopGoods[i].transform.childCount == 0)
            {
                //없을경우 -> 오브젝트 생성후 피스의 아이템 데이타 랜덤 돌린거 넣어주기
                goodsPiece = Instantiate(prefab, shopGoods[i].transform).GetComponentInChildren<Piece>();
                goodsPiece.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                goodsPiece = shopGoods[i].GetComponentInChildren<Piece>();
            }

            //Debug.Log(goodsPiece.gameObject.name);

            goodsPiece.ChangeItemData(itemDatas[rand]);
        }
    }

    public void Select(int CharNum)
    {
        ApplyItem(itemDatas[CharNum]);
    }

    public void ApplyItem(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //플레이어 오브젝트 자식으로 웨폰의 아이템 타입이 있는지 확인
                Weapon[] weapons = GameManager.instance.player.gameObject.GetComponentsInChildren<Weapon>();

                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i].id == itemData.itemId)
                    {
                        weapon = weapons[i];
                        break;
                    }
                }

                if (weapon == null)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();

                    weapon.Init(itemData);
                }
                else
                {
                    weapon.CountUp();
                }
                itemData.itemQuantity++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                Gear[] gears = GameManager.instance.player.gameObject.GetComponentsInChildren<Gear>();

                for (int i = 0; i < gears.Length; i++)
                {
                    if (gears[i].type == itemData.itemType)
                    {
                        gear = gears[i];
                        break;
                    }
                }

                if (gear==null)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(itemData);
                }
                else
                {
                    gear.CountUp();
                }
                itemData.itemQuantity++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }
    }

    public void RemoveItem(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //플레이어 오브젝트 자식으로 웨폰의 아이템 타입이 있는지 확인
                Weapon[] weapons = GameManager.instance.player.gameObject.GetComponentsInChildren<Weapon>();

                for (int i = 0; i < weapons.Length; i++)
                {
                    if (weapons[i].id == itemData.itemId)
                    {
                        weapon = weapons[i];
                    }
                }

                if (weapon==null)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();

                    weapon.Init(itemData);
                }
                else
                {
                    weapon.CountUp();
                }
                itemData.itemQuantity--;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                Gear[] gears = GameManager.instance.player.gameObject.GetComponentsInChildren<Gear>();

                for (int i = 0; i < gears.Length; i++)
                {
                    if (gears[i].type == itemData.itemType)
                    {
                        gear = gears[i];
                        break;
                    }
                }

                if (gear == null)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(itemData);
                }
                else
                {
                    gear.CountDown();
                }
                itemData.itemQuantity--;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }
    }

}