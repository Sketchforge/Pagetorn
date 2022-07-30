using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour
{
    [SerializeField] private Image _slot;
    [SerializeField] private GameObject _selected;
    [SerializeField] private bool _filterItemType;
    [SerializeField, ShowIf("_filterItemType")] private ItemType _filter;
    [SerializeField] private Item _item;
    [SerializeField] private int _amount;
    
    public Action OnItemUpdate = delegate { };

    public bool HasItem => _item != null;
    public Item Item => _item;
    public (Item, int) GetItem() => (_item, _amount);
    public bool AllowsItem(Item item) => !_filterItemType || _filter == item.Type;

    [Button]
    public void UpdateItemSlot()
    {
        if (_item == null)
        {
            _amount = 0;
            _slot.gameObject.SetActive(false);
        }
        else
        {
            _slot.sprite = _item.Sprite;
            _slot.gameObject.SetActive(true);
        }
        OnItemUpdate?.Invoke();
    }

    public void InsertItem(Item item, int amount)
    {
        _item = item;
        _amount = amount;
        UpdateItemSlot();
    }

    public (Item, int) SwapItem(Item item, int amount)
    {
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
        _selected.SetActive(selected);
    }
}