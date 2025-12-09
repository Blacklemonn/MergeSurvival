using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        //Basic Set
        name = "Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localScale = Vector3.zero;

        //Property Set
        type = data.itemType;
        rate = data.baseDamage;

        ApplyGear();
    }

    public void CountUp()
    {
        this.rate += rate;
        ApplyGear();
    }

    private void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    private void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    float speed = 150 * Character.WeaponSpeed;
                    weapon.speed = 150 + (150 * rate);
                    break;
                default:
                    speed = 0.5f * Character.WeaponRate;
                    weapon.speed = 0.5f * (1f - rate);
                    break;
            }
        }
    }
    private void SpeedUp()
    {
        float speed = Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
