using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	[SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _heldItemParent;
    [SerializeField] private Image _heldItemImage;
    [SerializeField] private TextMeshProUGUI _heldItemAmountText;
    [SerializeField] private List<InventoryItemSlot> _slots;

    private Item _heldItem;
    private int _heldItemAmount;

    public bool MovingItem { get; private set; }

    private void Start()
    {
        if (!_canvas) Debug.LogWarning($"Missing Inventory Container (GameObject) for {name}", gameObject);
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
	    if (_canvas) _canvas.enabled = active;
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
        if (slot == null) slot = _slots.FirstOrDefault(s => s.CanPlaceItem(item, amount));
        if (slot == null) return false;

        var isFirstItem = false;
        if (!slot.HasItem) isFirstItem = true;

        slot.InsertItem(item, amount);

        if (isFirstItem && slot == CanvasController.ToolbarManager.SelectedItemSlot)
            CanvasController.ToolbarManager.UpdateSlot();

        return true;
    }

    public void TryDetailItem(InventoryItemSlot slot)
    {
        //Debug.Log("Detailing item test");
        if (!slot.HasItem)
        {
            CanvasController.Singleton.CloseDetails();
            return;
        }
        //Debug.Log("Can detail");
        CanvasController.Singleton.OpenDetails(slot.Item.ItemName, slot.Item.ItemDescription);

    }

    // Pickup item from Inventory UI
    public void TryPickupItem(InventoryItemSlot slot, bool left)
    {
        if (!slot.HasItem) return;
        if (_heldItem == null)
        {
            PickupItem(slot, left);
        }
        else
        {
            if (!left && slot.CanPlaceItem(_heldItem, _heldItemAmount))
            {
                PlaceItem(slot, true);
            }
            else
            {
                SwapItem(slot);
            }
        }
    }
    private void PickupItem(InventoryItemSlot slot, bool full = true)
    {
        (_heldItem, _heldItemAmount) = slot.GetItem();
        if (_heldItemAmount > 1 && !full)
        {
            _heldItemAmount = Mathf.CeilToInt(_heldItemAmount / 2f);
            slot.RemoveItem(_heldItemAmount);
            slot.UpdateItemSlot();
        }
        else
        {
            slot.ClearSlot();
        }
        UpdateHeldItem();

        if (slot == CanvasController.ToolbarManager.SelectedItemSlot)
            CanvasController.ToolbarManager.UpdateSlot();
    }

    // Swap item from Inventory UI
    private void SwapItem(InventoryItemSlot slot)
    {
        if (!slot.AllowsItem(_heldItem)) return;
        (_heldItem, _heldItemAmount) = slot.SwapItem(_heldItem, _heldItemAmount);
        UpdateHeldItem();
        if (slot == CanvasController.ToolbarManager.SelectedItemSlot)
            CanvasController.ToolbarManager.UpdateSlot();
    }
    
    // Place item from Inventory UI
    public void TryPlaceItem(InventoryItemSlot slot, bool left)
    {
        if (_heldItem == null) return;
        PlaceItem(slot, !left);
    }
    private void PlaceItem(InventoryItemSlot slot, bool onlyOne = false)
    {
        if (!slot.CanPlaceItem(_heldItem, _heldItemAmount))
        {
            Debug.LogError($"Attempting to place item ({_heldItem.ItemName}) in slot while slot ({slot.name}) cannot hold item", gameObject);
            return;
        }
        slot.InsertItem(_heldItem, onlyOne ? 1 : _heldItemAmount);
        _heldItemAmount -= onlyOne ? 1 : _heldItemAmount;
        if (_heldItemAmount <= 0) _heldItem = null;
        UpdateHeldItem();
        if (slot == CanvasController.ToolbarManager.SelectedItemSlot)
            CanvasController.ToolbarManager.UpdateSlot();
    }
}