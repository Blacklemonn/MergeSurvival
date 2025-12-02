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
    public int width; // 가로크기
    public int height; // 세로크기
    [TextArea]
    public string itemDesc;
    public Sprite itemIcon;
    public Sprite displayIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("# Weapon")]
    public GameObject projectile;
    public Sprite hand;
}
