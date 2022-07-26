using UnityEngine;

public abstract class Item : ScriptableObject
{
    [Header("Basic Item Info")]
    [SerializeField] private string _itemName;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private bool _canStack;
    [ShowIf("_canStack")] [SerializeField] private int _stackAmount = 99;

    public string ItemName => _itemName;
    public Sprite Sprite => _sprite;
    public bool CanStack => _canStack;
    public int StackAmount => _canStack ? _stackAmount : 1;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(ItemName))
        {
            _itemName = name;
        }
    }
}