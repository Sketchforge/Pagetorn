using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CoffeyUtils;

public class ToolbarSlot : MonoBehaviour
{
    [SerializeField] private Image _slot;
	[SerializeField] private Image _selected;
	[SerializeField] private Color _selectedColor = Color.green;
	[SerializeField] private Color _notSelectedColor = new Color(0, 0, 0, 0);
	[SerializeField] private TMP_Text _amountText;
    [SerializeField] private GameObject _healthTicks;
    [SerializeField] private List<Image> _healthPoints;
    [SerializeField] private InventoryItemSlot _slotToCopy;
	
	private static Color _invisible = new Color(0,0,0,0);

    private void OnEnable()
    {
        _slotToCopy.OnItemUpdate += UpdateVisual;
        _slotToCopy.OnSelected += SetSelected;
        UpdateVisual();
	    _selected.color = _notSelectedColor;
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
        if (hasSprite)
        {
            _slot.sprite = item.Sprite;

            if (item.HasHealth)
            {
                _healthTicks.SetActive(true);

                for (int i = 0; i < _healthPoints.Count; i++)
                {
                    bool activeHealth = i < _slotToCopy.ItemHealth;
                    _healthPoints[i].color = activeHealth ? Color.green : Color.gray;
                }
            }

        }
        else if(!hasSprite)
        {
            if (!item) _healthTicks.SetActive(false);
        }
	    _amountText.text = amount > 1 ? amount.ToString() : "";
        


    }

    private void SetSelected(bool selected)
	{
		_selected.color = selected ? _selectedColor : _notSelectedColor;
    }
}
