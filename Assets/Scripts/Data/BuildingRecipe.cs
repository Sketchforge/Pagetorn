using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Building Recipe")]
public class BuildingRecipe : ScriptableObject
{
    [SerializeField] private List<ItemAmount> _recipe;
    [Header("Output Building")]
    [SerializeField] private string _buildingName;
    [SerializeField] private float _hp;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_buildingName))
        {
            _buildingName = name;
        }
    }
}
