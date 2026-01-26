using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    //플레이어의 무기를 저장할 변수
    private List<Weapon> weaponList;
    //플레이어의 기어를 저장할 변수
    private List<Gear> gearList;

    private List<InventorySlot> highlightSlot;

    private const int GridX = 5;
    private const int GridY = 5;

    private const int AddMaxHealth = 50;

    [HideInInspector]
    public InventorySlot[,] grid;
    [HideInInspector]
    public Vector2Int placeGrid;
    [Header("#Slot")]
    public RectTransform itemRoot;
    public GameObject inventory;

    public Sprite[] slotSprite = new Sprite[2];
    [Header("#MergeData")]
    public MergeRecipe[] mergeRecipes;

    private void Awake()
    {
        highlightSlot = new List<InventorySlot>();
        grid = new InventorySlot[GridX,GridX];
        placeGrid = Vector2Int.zero;
        weaponList = new List<Weapon>();
        gearList = new List<Gear>();

        for (int i = 0; i <GridX; i++)
        {
            for (int j = 0; j < GridY; j++)
            {
                grid[i,j] = inventory.transform.GetChild(i+(j*5)).GetComponent<InventorySlot>();
                grid[i,j].gridX = i;
                grid[i,j].gridY = j;
            }
        }
    }

    public InventorySlot GetPlaceGrid
    {
        get { return grid[placeGrid.x, placeGrid.y]; }
    }

    public void OccupySlots(Vector2Int start, ItemData item)
    {
        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                grid[start.x + x, start.y + y].HasItem = true;
            }
        }
    }

    public bool TryGetPlacePosition(InventorySlot hoverSlot, Vector2Int offset, ItemData item, out Vector2Int placePos)
    {
        int startX = hoverSlot.gridX - offset.x;
        int startY = hoverSlot.gridY - offset.y;

        placePos = new Vector2Int(startX, startY);

        // 범위 체크
        if (startX < 0 || startY < 0) return false;
        if (startX + item.width > GridX) return false;
        if (startY + item.height > GridY) return false;

        return true;
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
                    GameObject newWeapon = new();
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
            case ItemData.ItemType.Health:
                GameManager.instance.maxHealth += AddMaxHealth;
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

                Weapon weapon = new();

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
                Gear gear = new();

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
            case ItemData.ItemType.Health:
                //캐릭터의 최대 체력을 늘려주기
                GameManager.instance.maxHealth -= AddMaxHealth;
                break;
        }
    }

    public bool CanPlaceItem(Vector2Int start, ItemData item)
    {
        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                int checkX = start.x + x;
                int checkY = start.y + y;

                
                InventorySlot slot = grid[checkX, checkY];

                if (item.itemType == ItemData.ItemType.Bag)
                {
                    if (slot.HasBag) return false;
                }
                else
                {
                    if (slot.HasItem || !slot.HasBag) return false;
                }
            }
        }

        return true;
    }

    public void PlaceItem(Vector2Int start, ItemData item, out List<InventorySlot> slots)
    {
        slots = new List<InventorySlot>();
        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                int checkX = start.x + x;
                int checkY = start.y + y;

                InventorySlot slot = grid[checkX, checkY];

                if (item.itemType == ItemData.ItemType.Bag)
                {
                    //슬롯의 sprite를 panel로 변경
                    slot.GetComponent<Image>().sprite = slotSprite[1];
                    //슬롯의 hasBag를 true로 변경
                    slot.HasBag = true;
                    //슬롯에 스택으로 저장해놓기
                    slots.Add(slot);
                }
                else
                {
                    //슬롯의 hasItem을 true로 변경
                    slot.HasItem = true;
                    slots.Add(slot);
                }
            }
        }
    }
    
    public void HighlightSlot(Vector2Int start, ItemData item, bool canPlace)
    {
        ClearHighlightSlot();

        for (int y = 0; y < item.height; y++)
        {
            for (int x = 0; x < item.width; x++)
            {
                int px = start.x + x;
                int py = start.y + y;
                if (px < 0 || py < 0) continue;
                if (px > 4 || py > 4) continue;
                //하이라이트된 슬롯 저장
                grid[px, py].SetHighlight(canPlace);
                highlightSlot.Add(grid[px, py]);
            }
        }
    }

    public void ClearHighlightSlot()
    {
        //이전에 하이라이트 되었던 슬롯 리셋
        foreach (var slot in highlightSlot)
        {
            Image image = grid[slot.gridX, slot.gridY].GetComponent<Image>();
            image.color = Color.white; // 기본색
        }
        highlightSlot.Clear();
    }

    public void HighlightItem(ItemData data)
    {
        foreach(GameObject obj in GetMergeItem(data))
        {
            obj.GetComponent<Image>().color = Color.red;
        }
    }

    public void ClearHighlightItem()
    {
        for (int i = 0; i < itemRoot.childCount; i++)
        {
            //가방일경우 넘기기
            if (itemRoot.GetChild(i).GetComponent<Piece>().itemData.itemType == ItemData.ItemType.Bag)
                continue;

            itemRoot.GetChild(i).GetComponent<Image>().color = Color.white;
        }
    }

    //인벤토리 안의 아이템들 중 data와 합쳐질 수 있는 아이템 반환
    private List<GameObject> GetMergeItem(ItemData data)
    {
        //현재 내가 가지고 있는 아이템
        List<GameObject> items = new List<GameObject>();
        //이 아이템이 될 아이템의 필요 재료들
        List<ItemData> itemIngredients = new List<ItemData>();
        //최종적으로 합쳐질 수 있는 아이템들
        List<GameObject> mergeItems = new List<GameObject>();
            
        //현재 어떤 아이템을 갖고 있는지
        for (int i = 0; i < itemRoot.childCount; i++)
        {
            //가방일경우 넘기기
            if (itemRoot.GetChild(i).GetComponent<Piece>().itemData.itemType == ItemData.ItemType.Bag)
                continue;
            
            items.Add(itemRoot.GetChild(i).gameObject);
        }

        //레시피에서 이 아이템이 될 수 있는것의 재료 가져오기
        for (int i = 0; i < data.resultRecipe.Length; i++)
        {
            //내가 주 재료일때 레시피에서 자신을 제외한 모든 데이터 추가
            if (data.resultRecipe[i].inputs[0].item == data)
            {
                foreach (Ingredient ingredient in data.resultRecipe[i].inputs)
                {
                    for (int j = 0; j < ingredient.count; j++)
                    {
                        itemIngredients.Add(ingredient.item);
                    }
                }
                itemIngredients.Remove(data);
            }
            //내가 부 재료일때 레시피의 주 재료만 추가
            else
            {
                itemIngredients.Add(data.resultRecipe[i].inputs[0].item);
            }
        }

        //내가 들고있는 아이템 중에서 합쳐질 수 있는 아이템들
        foreach (GameObject obj in items)
        {
            foreach (ItemData ingredient in itemIngredients)
            {
                if (obj.GetComponent<Piece>().itemData == ingredient)
                {
                    mergeItems.Add(obj);
                    break;
                }
            }
        }

        return mergeItems;
    }

    public MergeRecipe CanMergeItem(ItemData data, Dictionary<ItemData,List<Piece>> nearItemDatas, bool Second)
    {
        bool canMerge = true;
        //data의 recipe개수만큼 반복
        foreach (MergeRecipe recipe in data.resultRecipe)
        {
            //아이템이 recipe의 주 재료인지 부재료인지
            if (recipe.inputs[0].item == data || recipe.inputs.Count <= 2)
            {
                //재료의 개수가 충분한지 확인
                for (int i = 0; i < recipe.inputs.Count; i++)
                {
                    if (recipe.inputs[i].count <= nearItemDatas[recipe.inputs[i].item].Count)
                    {
                        continue;
                    }
                    else
                    {
                        canMerge = false;
                        break;
                    }
                }
                if (canMerge)
                    return recipe;
            }
            else if(!Second)
            {
                //근처아이템에 주 재료가 되는 아이템이 있는지
                if (nearItemDatas.ContainsKey(recipe.inputs[0].item))
                {
                    //근처의 메인 재료의 Piece를 가져와서 그것의 TryItemMerge을 실행
                    nearItemDatas[recipe.inputs[0].item][0].TryItemMerge(true);
                }
                else { }
            }
        }
        return null;
    }
}