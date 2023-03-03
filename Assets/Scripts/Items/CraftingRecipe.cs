using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Output Item")]
    [SerializeField] private ItemAmount _output = new ItemAmount(1);
    [SerializeField] private List<ItemAmount> _ingredients = new List<ItemAmount>();

    public ItemAmount Output => _output;
    public List<ItemAmount> Ingredients => _ingredients;
}