using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalStatDisplay : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _displayText;
    [SerializeField] private SurvivalStat _stat;

    private void OnEnable()
    {
        _stat.OnChange += UpdateStat;
    }

    private void OnDisable()
    {
        _stat.OnChange -= UpdateStat;
    }

    private void Start()
    {
        if (_slider) _slider.maxValue = _stat.Max;
        UpdateStat(_stat.Value);
    }

    private void UpdateStat(float value)
    {
        if (_slider) _slider.value = _stat.Value;
        if (_displayText) _displayText.text = _stat.Value.ToString("F0");
    }
}