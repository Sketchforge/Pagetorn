using System.Collections.Generic;
using UnityEngine;

public class Inventory : ScriptableObject
{
    [SerializeField] private List<ItemAmount> _heldItems;
}
