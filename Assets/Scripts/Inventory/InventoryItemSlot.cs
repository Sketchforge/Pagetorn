using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour
{
    [SerializeField] private Image _slot;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private GameObject _selected;
    [SerializeField] private bool _filterItemType;
    [SerializeField, ShowIf("_filterItemType")] private ItemType _filter;
    [SerializeField] private Item _item;
    [SerializeField] private int _amount;
    
    public Action OnItemUpdate = delegate { };
    public Action<bool> OnSelected = delegate { };

    public bool HasItem => _item != null;
    public Item Item => _item;
    public (Item, int) GetItem() => (_item, _amount);
    public bool AllowsItem(Item item) => !_filterItemType || _filter == item.Type;
    public bool HasSpaceForItem(int amount) => _amount + amount < _item.StackAmount;
    public bool CanStackItem(Item item, int amount) => HasItem && _item.Equals(item) && HasSpaceForItem(amount);
    public bool CanPlaceItem(Item item, int amount) => HasItem ? _item.Equals(item) && HasSpaceForItem(amount) : AllowsItem(item);
    public bool Selected { get; private set; }

    private void Start()
    {
        if (!_slot) Debug.LogWarning($"Missing Slot (Image) for {name}", gameObject);
        if (!_amountText) Debug.LogWarning($"Missing Amount Text for {name}", gameObject);
        if (!_selected) Debug.LogWarning($"Missing Selected Object for {name}", gameObject);
    }

    [Button]
    public void UpdateItemSlot()
    {
        if (_item == null)
        {
            _amount = 0;
            if (_slot) _slot.gameObject.SetActive(false);
            if (_amountText) _amountText.gameObject.SetActive(false);
        }
        else
        {
            if (_slot)
            {
                _slot.sprite = _item.Sprite;
                _slot.gameObject.SetActive(true);
            }
            if (_amountText)
            {
                bool moreThanOne = _amount > 1;
                _amountText.gameObject.SetActive(moreThanOne);
                if (moreThanOne) _amountText.text = _amount.ToString();
            }
        }
        OnItemUpdate?.Invoke();
    }

    public void InsertItem(Item item, int amount)
    {
        if (HasItem && !_item.Equals(item))
        {
            Debug.LogError($"Attempting to place item ({item.ItemName}) in slot while slot is holding different item ({_item.ItemName})", gameObject);
            return;
        }
        if (HasItem)
        {
            _amount += amount;
        }
        else
        {
            _item = item;
            _amount = amount;
        }
        UpdateItemSlot();
    }

    public (Item, int) SwapItem(Item item, int amount)
    {
        if (item.Equals(_item) && HasSpaceForItem(amount))
        {
            _amount += amount;
            return (null, 0);
        }
        var temp = (_item, _amount);
        InsertItem(item, amount);
        return temp;
    }

    public void ClearSlot()
    {
        _item = null;
        UpdateItemSlot();
    }

    public void OnClickSlot()
    {
        if (_item == null)
        {
            CanvasController.InventoryManager.TryPlaceItem(this);
        }
        else
        {
            CanvasController.InventoryManager.TryPickupItem(this);
        }
    }

    public void SetSelected(bool selected)
    {
        if (_selected) _selected.SetActive(selected);
        Selected = selected;
        OnSelected?.Invoke(selected);
    }
}