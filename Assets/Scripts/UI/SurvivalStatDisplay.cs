using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalStatDisplay : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _displayText;
    [SerializeField] private Survival _survival;
    [SerializeField] private SurvivalStatEnum _stat;

    // TODO: Functionality for when stats are low (_survival.IsStatLow(_stat))
    
    private void OnEnable()
    {
        _survival.OnStatsChanged += UpdateStat;
        UpdateStat();
    }

    private void OnDisable()
    {
        _survival.OnStatsChanged -= UpdateStat;
    }

    private void Start()
    {
        if (_slider) _slider.maxValue = _survival.GetStatMax(_stat);
    }

    private void UpdateStat()
    {
        float value = _survival.GetStat(_stat);
        if (_slider) _slider.value = value;
        if (_displayText) _displayText.text = value.ToString("F0");
    }
}