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

    private void OnEnable()
    {
        _slotToCopy.OnItemUpdate += UpdateVisual;
        _slotToCopy.OnSelected += SetSelected;
        UpdateVisual();
        _selected.SetActive(_slotToCopy.Selected);
    }
    
    private void OnDisable()
    {
        _slotToCopy.OnItemUpdate += UpdateVisual;
        _slotToCopy.OnSelected -= SetSelected;
    }

    [Button]
    private void UpdateVisual()
    {
        (Item item, int amount) = _slotToCopy.GetItem();
        bool hasSprite = item && item.Sprite;
        _slot.gameObject.SetActive(hasSprite);
        if (hasSprite) _slot.sprite = item.Sprite;
    }

    private void SetSelected(bool selected)
    {
        _selected.SetActive(selected);
    }
}
