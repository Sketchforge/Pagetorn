using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Item Info")]
    [SerializeField, ReadOnly] private string _itemName;
    [SerializeField] private ItemType _type = ItemType.Basic;
    [SerializeField, TextArea] private string _description = "";
    [SerializeField] private Sprite _sprite;
    [SerializeField, ReadOnly] private bool _canStack;
    [ShowIf("_canStack")] [SerializeField] private int _stackAmount = 99;

    public string ItemName => _itemName;
    public ItemType Type => _type;
    public Sprite Sprite => _sprite;
    public bool CanStack => _canStack;
    public int StackAmount => _canStack ? _stackAmount : 1;

    private void OnValidate()
    {
        _itemName = name.Replace("SC", "SoftCover").Replace("HC", "HardCover").Replace(" ", "");
        _canStack = !(IsArmor || IsToolOrWeapon);
    }

    public bool IsArmor => _type is ItemType.Leggings or ItemType.Chestplate or ItemType.Helmet;
    public bool IsToolOrWeapon => _type is ItemType.Blade or ItemType.Hammer or ItemType.Tool;
}