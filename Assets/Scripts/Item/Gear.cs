using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;
    
    public float baseRate;

    public void Init(ItemData data)
    {
        //Basic Set
        name = "Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localScale = Vector3.zero;

        //Property Set
        type = data.itemType;
        baseRate = data.baseDamage;
        rate = baseRate;
        data.baseCount = 1;

        ApplyGear();
    }

    public void CountUp()
    {
        this.rate += baseRate;
        ApplyGear();
    }
    public void CountDown()
    {
        this.rate -= baseRate;
        ApplyGear();
    }

    private void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                Rate();
                break;
            case ItemData.ItemType.Shoe:
                Speed();
                break;
            case ItemData.ItemType.Bullet:
                Damage();
                break;
        }
    }

    private void Damage()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        //원거리 무기에만 데미지 변화
        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 1:
                case 5:
                case 6:
                    
                    break;
            }
        }
    }

    private void Rate()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                case 7:
                case 8:
                    float speed = weapon.speed * Character.WeaponSpeed;
                    weapon.speed = weapon.speed + (weapon.speed * rate);
                    break;

                default:
                    speed = 0.5f * Character.WeaponRate;
                    weapon.speed = 0.5f * (1f - rate);
                    break;
            }
        }
    }
    private void Speed()
    {
        float speed = Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
