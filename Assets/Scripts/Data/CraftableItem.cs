using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Craftable Item")]
public class CraftableItem : ScriptableObject
{
    [SerializeField] private List<ItemAmount> _recipe;
    [Header("Output Item")]
    [SerializeField] private ItemAmount _outputItem;
}