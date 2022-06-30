using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Player/Survival Stat")]
public class SurvivalStat : ScriptableObject
{
    [SerializeField, ReadOnly] private float _value;
    [SerializeField] private float _maxValue = 100;

    public event Action<float> OnChange = delegate { };
    public event Action<float> OnDecrease = delegate { };
    public event Action OnReachZero = delegate { };

    public void SetMaxHealth() {
        _value = _maxValue;
        OnChange?.Invoke(_value);
    }

    public void IncreaseHealth(float amount) {
        _value += amount;
        if (_value > _maxValue) _value = _maxValue;
        OnChange?.Invoke(_value);
    }

    public void DecreaseHealth(float amount) {
        _value -= amount;
        if (_value < 0) _value = 0;
        OnChange?.Invoke(_value);

        if (_value <= 0) OnReachZero?.Invoke();
        else OnDecrease?.Invoke(amount);
    }
}
