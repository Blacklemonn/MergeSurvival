using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //인벤토리 슬롯에 있는 아이템을 가져올 변수
    public ItemData[] slotItem;
    //플레이어의 무기를 저장할 변수
    public List<Weapon> weaponList;
    //플레이어의 기어를 저장할 변수
    public List<Gear> gearList;
    
    private void Awake()
    {
        slotItem = new ItemData[transform.childCount];
    }

    //아이템을 플레이어에게 적용시켜주는 함수
    public void ApplyItem(ItemData itemData, bool UseMoney)
    {
        GameManager.instance.UseMoney(itemData.itemPrice, UseMoney);

        switch (itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //플레이어 오브젝트 자식으로 웨폰의 아이템 타입이 있는지 확인
                
                Weapon weapon = null;

                for (int i = 0; i < weaponList.Count; i++)
                {
                    if (weaponList[i].id == itemData.itemId)
                    {
                        weapon = weaponList[i];
                        break;
                    }
                }

                if (weapon == null)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(itemData);
                    weaponList.Add(weapon);
                }
                else
                {
                    if(!weapon.gameObject.activeSelf)
                        weapon.gameObject.SetActive(true);

                    weapon.CountUp();
                }
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:

                Gear gear = null;

                for (int i = 0; i < gearList.Count; i++)
                {
                    if (gearList[i].type == itemData.itemType)
                    {
                        gear = gearList[i];
                        break;
                    }
                }

                if (gear == null)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(itemData);
                    gearList.Add(gear);
                }
                else
                {
                    if (!gear.gameObject.activeSelf)
                        gear.gameObject.SetActive(true);

                    gear.CountUp();
                }
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                //heal오브젝트가 사라져야함.

                break;
        }
    }

    //플레이어의 아이템을 제거하는 함수
    public void RemoveItem(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //플레이어 오브젝트 자식으로 웨폰의 아이템 타입이 있는지 확인

                Weapon weapon = new Weapon();

                for (int i = 0; i < weaponList.Count; i++)
                {
                    if (weaponList[i].id == itemData.itemId)
                    {
                        weapon = weaponList[i];
                        break;
                    }
                    else
                    {
                        weapon = null;
                    }
                }

                weapon.CountDown();

                if (weapon.count == 0)
                    weapon.gameObject.SetActive(false);

                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                Gear gear = new Gear();

                for (int i = 0; i < gearList.Count; i++)
                {
                    if (gearList[i].type == itemData.itemType)
                    {
                        gear = gearList[i];
                        break;
                    }
                    else
                    {
                        gear = null;
                    }
                }

                gear.CountDown();

                if (gear.rate <= gear.baseRate)
                    gear.gameObject.SetActive(false);
                break;
        }
    }

    //
}