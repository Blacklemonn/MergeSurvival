using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Health, Bullet, Bag }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public int itemPrice; 
    public int width; // 가로크기
    public int height; // 세로크기

    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float baseRate;

    [Header("# Weapon")]
    public GameObject projectile;
    public Sprite hand;

    [Header("# Merge")]
    //될 수 있는 아이템의 레시피
    public MergeRecipe[] resultRecipe;
}
