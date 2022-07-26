using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Transform _heldItemParent;
    [SerializeField] private Image _heldItemImage;
    [SerializeField] private List<InventoryItemSlot> _slots;

    private Item _heldItem;
    private int _heldItemAmount;

    public bool MovingItem { get; private set; }

    private void Start()
    {
        foreach (var slot in _slots)
        {
            slot.UpdateItemSlot();
        }
    }

    private void Update()
    {
        if (MovingItem)
        {
            _heldItemParent.position = PlayerInputManager.MousePos;
        }
    }

    public void SetActive(bool active) => _container.SetActive(active);

    private void UpdateHeldItem()
    {
        if (_heldItem != null)
        {
            MovingItem = true;
            _heldItemParent.gameObject.SetActive(true);
            _heldItemImage.sprite = _heldItem.Sprite;
        }
        else
        {
            MovingItem = false;
            _heldItemParent.gameObject.SetActive(false);
        }
    }

    // Add item from external source (Not UI -- picking up item from world or other)
    public void AddItemToInventory(Item item, int amount)
    {
        // TODO: Add item to inventory slots
    }

    // Pickup item from Inventory UI
    public void TryPickupItem(InventoryItemSlot slot)
    {
        if (_heldItem == null)
        {
            PickupItem(slot);
        }
        else
        {
            SwapItem(slot);
        }
    }
    private void PickupItem(InventoryItemSlot slot)
    {
        (_heldItem, _heldItemAmount) = slot.GetItem();
        slot.ClearSlot();
        UpdateHeldItem();
    }

    // Swap item from Inventory UI
    private void SwapItem(InventoryItemSlot slot)
    {
        (_heldItem, _heldItemAmount) = slot.SwapItem(_heldItem, _heldItemAmount);
        UpdateHeldItem();
    }
    
    // Place item from Inventory UI
    public void TryPlaceItem(InventoryItemSlot slot)
    {
        if (_heldItem != null)
        {
            PlaceItem(slot);
        }
    }
    private void PlaceItem(InventoryItemSlot slot)
    {
        slot.InsertItem(_heldItem, _heldItemAmount);
        _heldItem = null;
        _heldItemAmount = 0;
        UpdateHeldItem();
    }
}