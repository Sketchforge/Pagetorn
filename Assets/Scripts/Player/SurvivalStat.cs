using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/PlayerManager/Survival Stat")]
public class SurvivalStat : ScriptableObject
{
    [SerializeField, ReadOnly] private float _value;
    [SerializeField] private float _maxValue = 100;

    public float Value => _value;
    public float Max => _maxValue;
    public event Action<float> OnChange = delegate { };
    public event Action<float> OnDecrease = delegate { };
    public event Action OnReachZero = delegate { };

    public void SetToMax()
    {
        _value = _maxValue;
        OnChange?.Invoke(_value);
    }

    [Button]
    public void Increase(float amount)
    {
        _value += amount;
        if (_value > _maxValue) _value = _maxValue;
        OnChange?.Invoke(_value);
    }

    [Button]
    public void Decrease(float amount)
    {
        _value -= amount;
        if (_value < 0) _value = 0;
        OnChange?.Invoke(_value);

        if (_value <= 0) OnReachZero?.Invoke();
        else OnDecrease?.Invoke(amount);
    }
}