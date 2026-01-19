using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Merge/MergeRecipe")]
public class MergeRecipe : ScriptableObject
{
    public ItemData result;
    public int resultTier;

    public List<Ingredient> inputs;
}

[System.Serializable]
public class Ingredient
{
    public ItemData item;
    public int tier;
    public int count;
}
