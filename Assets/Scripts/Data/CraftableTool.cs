using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Craftable Tool")]
public class CraftableTool : ScriptableObject
{
    [SerializeField] private List<ItemAmount> _recipe;
    [Header("Output Tool")]
    [SerializeField] private string _toolName;
    [SerializeField] private float _damage;
    [SerializeField] private float _knockback;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_toolName))
        {
            _toolName = name;
        }
    }
}
