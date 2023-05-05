using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : PlayerInteractable
{
    [SerializeField] private List<CraftingRecipe> _availableRecipes = new List<CraftingRecipe>();
    
    public override void Interact()
    {
        CanvasController.Singleton.OpenCrafting();
    }
}