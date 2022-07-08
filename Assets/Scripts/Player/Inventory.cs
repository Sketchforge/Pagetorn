using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/PlayerManager/Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private List<ItemAmount> _heldItems;


    // Need to determine whether Buildings, Tools, etc. are items in inventory
    // The alternative is building like Satisfactory or other games where you just need the raw material and can build it instead of 
    // crafting a building item. I think this makes more sense because we are dealing with books and other small objects that make up a building
    // As for tools and armor this could go either way
}