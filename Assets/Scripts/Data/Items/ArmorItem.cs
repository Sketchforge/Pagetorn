using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Items/Armor")]
public class ArmorItem : Item
{
    [Header("Armor Specific")]
    [SerializeField] private int _protection = 10;

    public int Protection => _protection;
}