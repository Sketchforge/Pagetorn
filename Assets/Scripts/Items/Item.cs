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
    [SerializeField, ReadOnly] private bool _hasHealth;
    [ShowIf("_hasHealth")] [SerializeField] private int _maxHealth = 5;
    [SerializeField] public GameObject _prefabObject;

    public string ItemName => _itemName;
    public ItemType Type => _type;
    public Sprite Sprite => _sprite;
    public int StackAmount => _canStack ? _stackAmount : 1;
    public bool HasHealth => _hasHealth;
    public int MaxHealth => _hasHealth ? _maxHealth + 1 : 0;
    public bool Equals(Item item) => item != null && _itemName.Equals(item.ItemName);
    public bool IsArmor => _type is ItemType.Leggings or ItemType.Chestplate or ItemType.Helmet;
    public bool IsToolOrWeapon => _type is ItemType.Blade or ItemType.Hammer or ItemType.Tool;

    protected virtual void OnValidate()
    {
        _itemName = name.Replace("SC", "SoftCover").Replace("HC", "HardCover").Replace(" ", "");
        _canStack = !(IsArmor || IsToolOrWeapon);
    }

    [Button(Mode = ButtonMode.InPlayMode, Spacing = 10)]
    public void AddItemToInventory()
    {
        CanvasController.InventoryManager.AddItemToInventory(this, 1);
    }
}