using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalStatDisplay : MonoBehaviour
{
    [SerializeField] private Survival _survival;
	[SerializeField] private SurvivalStatEnum _stat;
    
	[Header("References")]
	[SerializeField] private TMP_Text _displayText;
	[SerializeField] private Slider _slider;
	[SerializeField] private CircleSlider _circleSlider;
	
	private float _max = 100;

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
		_max = _survival.GetStatMax(_stat);
		if (_slider) _slider.maxValue = _max;
    }

    private void UpdateStat()
    {
        float value = _survival.GetStat(_stat);
        if (_slider) _slider.value = value;
	    if (_circleSlider) _circleSlider.UpdateSlider(value / _max);
        if (_displayText) _displayText.text = value.ToString("F0");
    }
}