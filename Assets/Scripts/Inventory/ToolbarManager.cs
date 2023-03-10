using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
	[SerializeField] private bool _scrollToBlankSlots;
    [SerializeField] private List<InventoryItemSlot> _slots = new List<InventoryItemSlot>();

    public InventoryItemSlot SelectedItemSlot => _slots[_selectedItem];
    public Item SelectedItem => _slots[_selectedItem].Item;
    public ItemType SelectedItemType => SelectedItem != null ? SelectedItem.Type : ItemType.Basic;

    [SerializeField] private Transform _playerObjectSocket;
    private GameObject objectSpawned;

    private int _selectedItem;

    private void Start()
    {
        SelectItem(0);
    }

    public void SelectNextItem()
    {
        for (int i = _selectedItem + 1; i < _slots.Count; i++)
        {
	        if (_scrollToBlankSlots || _slots[i].HasItem)
            {
                SelectItem(i);
                return;
            }
        }
        for (int i = 0; i < _selectedItem; i++)
        {
	        if (_scrollToBlankSlots || _slots[i].HasItem)
            {
                SelectItem(i);
                if (objectSpawned)
                    Destroy(objectSpawned);//for future reference, because its 1 am, should instead deactivate objects, and check if one exists, not destroy the object. Or destroy object but save data.
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
	        if (_scrollToBlankSlots || _slots[i].HasItem)
            {
                SelectItem(i);
                return;
            }
        }
        for (int i = _slots.Count - 1; i > _selectedItem; i--)
        {
	        if (_scrollToBlankSlots || _slots[i].HasItem)
            {
                SelectItem(i);
                if (objectSpawned)
                    Destroy(objectSpawned);
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
        bool hasPrefabObject = SelectedItem && SelectedItem._prefabObject;
        if (hasPrefabObject)
        {
           objectSpawned = Instantiate(SelectedItem._prefabObject, _playerObjectSocket);
            //objectSpawned.transform.position = new Vector3(0, 0, 0);
        }
    }
}
