using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _container;
    [SerializeField] private Transform _heldItemParent;
    [SerializeField] private Image _heldItemImage;
    [SerializeField] private TextMeshProUGUI _heldItemAmountText;
    [SerializeField] private List<InventoryItemSlot> _slots;

    private Item _heldItem;
    private int _heldItemAmount;

    public bool MovingItem { get; private set; }

    private void Start()
    {
        if (!_container) Debug.LogWarning($"Missing Inventory Container (GameObject) for {name}", gameObject);
        if (!_heldItemParent) Debug.LogWarning($"Missing Held Item Container (GameObject) for {name}", gameObject);
        if (!_heldItemImage) Debug.LogWarning($"Missing Held Item Icon (Image) for {name}", gameObject);
        if (!_heldItemAmountText) Debug.LogWarning($"Missing Held Item Amount (Text) for {name}", gameObject);
        foreach (var slot in _slots)
        {
            if (slot) slot.UpdateItemSlot();
        }
    }

    private void Update()
    {
        if (MovingItem)
        {
            _heldItemParent.position = PlayerInputManager.MousePos;
        }
    }

    public void SetActive(bool active)
    {
        if (!active && MovingItem)
        {
            if (AddItemToInventory(_heldItem, _heldItemAmount))
            {
                _heldItem = null;
                _heldItemAmount = 0;
            }
            UpdateHeldItem();
        }
        if (_container) _container.SetActive(active);
    }

    private void UpdateHeldItem()
    {
        if (_heldItem != null)
        {
            MovingItem = true;
            if (_heldItemParent) _heldItemParent.gameObject.SetActive(true);
            if (_heldItemImage) _heldItemImage.sprite = _heldItem.Sprite;
            if (_heldItemAmountText)
            {
                bool moreThanOne = _heldItemAmount > 1;
                _heldItemAmountText.gameObject.SetActive(moreThanOne);
                _heldItemAmountText.text = _heldItemAmount.ToString();
            }
        }
        else
        {
            MovingItem = false;
            if (_heldItemParent) _heldItemParent.gameObject.SetActive(false);
        }
    }

    // Add item from external source (Not UI -- picking up item from world or other)
    public bool AddItemToInventory(Item item, int amount)
    {
        var slot = _slots.FirstOrDefault(s => s.CanStackItem(item, amount));
        if (slot == null) slot = _slots.FirstOrDefault(s => s.AllowsItem(item));
        if (slot == null) return false;
        slot.InsertItem(item, amount);
        return true;
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
        if (!slot.AllowsItem(_heldItem)) return;
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
        if (!slot.CanPlaceItem(_heldItem, _heldItemAmount)) return;
        slot.InsertItem(_heldItem, _heldItemAmount);
        _heldItem = null;
        _heldItemAmount = 0;
        UpdateHeldItem();
    }
}