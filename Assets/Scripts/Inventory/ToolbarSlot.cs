﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolbarSlot : MonoBehaviour
{
    [SerializeField] private Image _slot;
	[SerializeField] private Image _selected;
	[SerializeField] private Color _selectedColor;
	[SerializeField] private TMP_Text _amountText;
	[SerializeField] private InventoryItemSlot _slotToCopy;
    
	private static Color _invisible = new Color(0, 0, 0, 0);

    private void OnEnable()
    {
        _slotToCopy.OnItemUpdate += UpdateVisual;
        _slotToCopy.OnSelected += SetSelected;
        UpdateVisual();
	    _selected.color = _invisible;
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
	    _slot.color = hasSprite ? Color.white : _invisible;
	    if (hasSprite) _slot.sprite = item.Sprite;
	    _amountText.text = amount > 1 ? amount.ToString() : "";
    }

    private void SetSelected(bool selected)
	{
		_selected.color = selected ? _selectedColor : _invisible;
    }
}
