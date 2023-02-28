using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSlider : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float _value;
    [SerializeField] private Color _valueColor = Color.green;
    
    [Header("Position")]
    [SerializeField, Range(0, 1)] private float _offset;
    [SerializeField, Range(0, 1)] private float _max;
    
    [Header("References")]
    [SerializeField] private Image _sliderBorder;
    [SerializeField] private Image _sliderBackground;
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _sliderFill;

    private void OnValidate()
    {
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        _slider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -_offset * 360f));
        _sliderBorder.fillAmount = _max + 0.005f;
        _sliderBackground.fillAmount = _max;
        _slider.value = _value * _max;
        _sliderFill.color = _valueColor;
    }
}
