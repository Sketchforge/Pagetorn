using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IDropHandler
{
    [SerializeField] private Image _slot;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private GameObject _selected;
    [SerializeField] private GameObject _healthPointsParent;
    [SerializeField] private List<Image> _healthPoints;
    [SerializeField] private bool _filterItemType;
    [SerializeField, ShowIf("_filterItemType")] private ItemType _filter;
    [SerializeField] private Item _item;
    [SerializeField] private int _amount;
    [SerializeField] private int _itemHealth;
    
    public Action OnItemUpdate = delegate { };
    public Action<bool> OnSelected = delegate { };

    public bool HasItem => _item != null;
    public bool HasSameItem(Item item) => _item.Equals(item);
    public Item Item => _item;
    public (Item, int) GetItem() => (_item, _amount);
    public bool AllowsItem(Item item) => !_filterItemType || _filter == item.Type;
    public int ItemHealth => _item != null ? (_item.HasHealth ? _itemHealth : 0) : 0;
    public bool HasSpaceForItem(int amount) => _amount + amount <= _item.StackAmount;
    public bool CanStackItem(Item item, int amount) => HasItem && HasSameItem(item) && HasSpaceForItem(amount);
    public bool CanPlaceItem(Item item, int amount) => HasItem ? HasSameItem(item) && HasSpaceForItem(amount) : AllowsItem(item);
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
            if (_amount == 0) _amount = 1;
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
            bool healthActive = _item.HasHealth && _itemHealth < _item.MaxHealth;
            _healthPointsParent.SetActive(healthActive);
            if (healthActive)
            {
                for (int i = 0; i < _healthPoints.Count; i++)
                {
                    bool activeHealth = i < _itemHealth;
                    _healthPoints[i].color = activeHealth ? Color.green : Color.gray;
                }
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
        if (HasSameItem(item) && HasSpaceForItem(amount))
        {
            _amount += amount;
            UpdateItemSlot();
            return (null, 0);
        }
        var temp = (_item, _amount);
        _item = null;
        InsertItem(item, amount);
        return temp;
    }
    
    public void DamageItem(int damage)
    {
        if (!HasItem)
        {
            Debug.LogError($"Trying to damage item that does not exist in slot ({name})!", gameObject);
            return;
        }
        if (!_item.HasHealth)
        {
            Debug.LogError($"Trying to damage item ({_item.ItemName}) that cannot take damage!", gameObject);
            return;
        }
        _itemHealth--;
        if (_itemHealth < 0)
        {
            RemoveItem(1);
            _itemHealth = _item ? _item.MaxHealth : 0;
        }
    }

    public void RemoveItem(int amount)
    {
        if (_amount <= amount)
        {
            ClearSlot();
            return;
        }
        _amount -= amount;
    }

    public void ClearSlot()
    {
        _item = null;
        _amount = 0;
        UpdateItemSlot();
    }

    public void SetSelected(bool selected)
    {
        if (_selected) _selected.SetActive(selected);
        Selected = selected;
        OnSelected?.Invoke(selected);
    }

    public void OnPointerClick(PointerEventData data)
    {
        bool left = data.button == PointerEventData.InputButton.Left;
        if (_item == null)
        {
            CanvasController.InventoryManager.TryPlaceItem(this, left);
        }
        else
        {
            CanvasController.InventoryManager.TryPickupItem(this, left);
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        bool left = data.button == PointerEventData.InputButton.Left;
        CanvasController.InventoryManager.TryPickupItem(this, left);
    }

    public void OnDrop(PointerEventData data)
    {
        bool left = data.button == PointerEventData.InputButton.Left;
        CanvasController.InventoryManager.TryPlaceItem(this, left);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }
}