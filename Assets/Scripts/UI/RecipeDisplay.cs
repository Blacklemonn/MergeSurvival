using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro를 사용하신다면 추가
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeDisplay : MonoBehaviour
{
    public MergeRecipe[] mergeRecipes;
    public GameObject ingredientPrefab; // 아이콘 이미지 프리팹
    public GameObject signTextPrefab;   // +, = 텍스트 프리팹
    public Transform contentParent;     // 이 한 줄들이 담길 부모 (Scroll View의 Content 등)
    public float space = 5f;           // 아이콘 사이의 간격

    private void Start()
    {
        foreach (MergeRecipe recipe in mergeRecipes)
        {
            if (recipe != null) Display(recipe);
        }
    }

    private void Display(MergeRecipe recipe)
    {
        // 1. 조합법 한 줄을 담을 부모 생성 (Horizontal Layout Group 포함)
        GameObject rowObj = new GameObject("RecipeRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        rowObj.transform.SetParent(contentParent, false);
        rowObj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        rowObj.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        rowObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        rowObj.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 20f);

        // 레이아웃 세팅
        HorizontalLayoutGroup layout = rowObj.GetComponent<HorizontalLayoutGroup>();
        layout.spacing = space;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        // 2. 재료 개수만큼 생성
        for (int i = 0; i < recipe.inputs.Count; i++)
        {
            // 재료 아이콘 생성 (Ingredient 클래스의 count만큼 반복 생성)
            for (int j = 0; j < recipe.inputs[i].count; j++)
            {
                CreateIcon(recipe.inputs[i].item.itemIcon, rowObj.transform);
                CreateSign("+", rowObj.transform);
            }
        }

        // 3. 마지막에 생성된 "+"를 "="로 변경
        if (rowObj.transform.childCount > 0)
        {
            Transform lastSign = rowObj.transform.GetChild(rowObj.transform.childCount - 1);
            TMP_Text textComp = lastSign.GetComponentInChildren<TMP_Text>();
            if (textComp != null) textComp.text = "=";
        }

        // 4. 결과물 아이콘 생성
        CreateIcon(recipe.result.itemIcon, rowObj.transform);
    }

    // 헬퍼 함수: 아이콘 생성
    private void CreateIcon(Sprite icon, Transform parent)
    {
        GameObject obj = Instantiate(ingredientPrefab, parent);
        obj.GetComponent<Image>().sprite = icon;
    }

    // 헬퍼 함수: 기호 텍스트 생성
    private void CreateSign(string sign, Transform parent)
    {
        GameObject obj = Instantiate(signTextPrefab, parent);
        obj.GetComponentInChildren<TMP_Text>().text = sign;
    }
}