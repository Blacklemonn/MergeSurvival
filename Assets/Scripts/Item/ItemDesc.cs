using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDesc : MonoBehaviour
{
    private ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textName;
    Text textDesc;

    private void Awake()
    {
        data = GetComponentInParent<Piece>().itemData;
        GetComponentsInChildren<Image>()[1].sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textName = texts[0];
        textDesc = texts[1];
        textName.text = data.itemName;
        textDesc.text = data.itemDesc;
    }

    public void ShowDesc()
    {
        gameObject.SetActive(true);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.baseDamage, data.baseCount);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.baseDamage);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            gameObject.SetActive(false);
        }
    }
}
