using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarSlot : MonoBehaviour
{
    [SerializeField] private Image _slot;
    [SerializeField] private GameObject _selected;
    [SerializeField] private InventoryItemSlot _slotToCopy;

    public bool Selectable => _slotToCopy.HasItem;

    public void SetSelected(bool selected)
    {
        _selected.SetActive(selected);
    }

    private void OnEnable()
    {
        UpdateVisual();
        _slotToCopy.OnItemUpdate += UpdateVisual;
    }
    
    private void OnDisable()
    {
        _slotToCopy.OnItemUpdate += UpdateVisual;
    }

    [Button]
    private void UpdateVisual()
    {
        (Item item, int amount) = _slotToCopy.GetItem();
        bool hasSprite = item && item.Sprite;
        _slot.gameObject.SetActive(hasSprite);
        if (hasSprite) _slot.sprite = item.Sprite;
    }
}
