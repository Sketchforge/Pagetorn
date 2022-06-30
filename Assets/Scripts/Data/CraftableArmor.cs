using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Craftable Armor")]
public class CraftableArmor : ScriptableObject
{
    [SerializeField] private List<ItemAmount> _recipe;
    [Header("Output Armor")]
    [SerializeField] private string _armorName;
    [SerializeField] private ArmorType _armorType;
    [SerializeField] private float _protection;

    private void OnValidate() {
        if (string.IsNullOrEmpty(_armorName)) {
            _armorName = name;
        }
    }
}
