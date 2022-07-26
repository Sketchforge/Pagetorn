using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    [SerializeField] private List<InventoryItemSlot> _slots = new List<InventoryItemSlot>();

    private int _selectedItem;

    private void Start()
    {
        SelectItem(0);
    }

    public void SelectNextItem()
    {
        for (int i = _selectedItem + 1; i < _slots.Count; i++)
        {
            if (_slots[i].HasItem)
            {
                SelectItem(i);
                return;
            }
        }
        for (int i = 0; i < _selectedItem; i++)
        {
            if (_slots[i].HasItem)
            {
                SelectItem(i);
                return;
            }
        }
        if (_slots[_selectedItem].HasItem) return;
        SelectItem((_selectedItem + 1) % _slots.Count);
    }

    public void SelectPreviousItem()
    {
        for (int i = _selectedItem - 1; i >= 0; i--)
        {
            if (_slots[i].HasItem)
            {
                SelectItem(i);
                return;
            }
        }
        for (int i = _slots.Count - 1; i > _selectedItem; i--)
        {
            if (_slots[i].HasItem)
            {
                SelectItem(i);
                return;
            }
        }
        if (_slots[_selectedItem].HasItem) return;
        if (_selectedItem == 0) SelectItem(_slots.Count - 1);
        else SelectItem(_selectedItem - 1);
    }

    public void SelectItem(int itemSlot)
    {
        _selectedItem = itemSlot;
        foreach (var slot in _slots)
        {
            slot.SetSelected(false);
        }
        _slots[itemSlot].SetSelected(true);
    }
}
