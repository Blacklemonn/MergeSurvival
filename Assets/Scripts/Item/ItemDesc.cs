using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemDesc : MonoBehaviour
{
    private ItemData data;
    public int level;
    public Weapon weapon;
    public Gear gear;

    public void ChangeDes(ItemData data)
    {
        Text[] texts = GetComponentsInChildren<Text>();
        Text textName = texts[0];
        Text textDesc = texts[1];
        Text textRecipe = texts[2];

        textName.text = data.itemName;

        // 1. 조합법 문자열 생성 로직
        if (data.resultRecipe != null && data.resultRecipe.Length > 0)
        {
            StringBuilder allRecipes = new StringBuilder();

            foreach (MergeRecipe recipe in data.resultRecipe)
            {
                List<string> ingredientNames = new List<string>();

                // 각 재료의 count만큼 이름을 리스트에 추가 (예: 삽 + 삽)
                foreach (Ingredient ingredient in recipe.inputs)
                {
                    for (int i = 0; i < ingredient.count; i++)
                    {
                        ingredientNames.Add(ingredient.item.itemName);
                    }
                }

                // " + "를 사이사이에 넣고 마지막에 " = 결과물" 추가
                string recipeString = string.Join(" + ", ingredientNames);
                recipeString += $" = {recipe.result.itemName}";

                allRecipes.AppendLine(recipeString); // 한 줄 추가
            }

            textRecipe.text = allRecipes.ToString();
        }
        else
        {
            textRecipe.text = "조합 불가"; // 조합법이 없는 경우
        }

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(data.itemDesc, data.baseDamage, data.baseCount, data.baseRate);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.baseDamage);
                break;
            default:
                textDesc.text = data.itemDesc; // 인자가 없는 경우 그냥 출력
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
