using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipe> _availableRecipes = new List<CraftingRecipe>();
}