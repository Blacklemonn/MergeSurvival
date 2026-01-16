using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal, Bag}

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

    [Header("# Weapon")]
    public GameObject projectile;
    public Sprite hand;

    [Header("# Merge")]
    //합치기 위해 필요한 아이템
    public ItemType[] mergeType;
    //테이블
    
}
