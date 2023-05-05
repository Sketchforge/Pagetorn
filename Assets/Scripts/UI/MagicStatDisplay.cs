using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagicStatDisplay : MonoBehaviour
{
    [SerializeField] private Survival _survival;
	[SerializeField] private SurvivalStatEnum _magicStat;
    
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
		_max = _survival.GetStatMax(_magicStat);
		if (_slider) _slider.maxValue = _max;
    }

    private void UpdateStat()
    {
        float value = _survival.GetStat(_magicStat);
        float[] levelCaps = _survival.LevelCaps.ToArray();
        float prevExp = 0;
        float nextExp = 1;
        float currentLevel = 1;

        for (int i = 0; i < levelCaps.Length; i++)
		{
            nextExp = levelCaps[i];
            if (value < nextExp)
                break;
            else
			{
                prevExp = levelCaps[i];
                currentLevel++;
			}
		}

        if (_slider)
        {
            _slider.maxValue = nextExp;
            _slider.minValue = prevExp;
            _slider.value = value;
        }
	    if (_circleSlider) _circleSlider.UpdateSlider(value / _max);
        if (_displayText) _displayText.text = "LV " + currentLevel.ToString();
    }
}