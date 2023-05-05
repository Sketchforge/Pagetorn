using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Survival Stats")]
public class Survival : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private float _health;
    [SerializeField] private float _healthLowThreshold = 20;
    [SerializeField] private float _maxHealth = 100;
    
    [Header("Hunger")]
    [SerializeField] private float _hunger;
    [SerializeField] private float _hungerLowThreshold = 20;
    [SerializeField] private float _maxHunger = 100;
    
    [Header("Hydration")]
    [SerializeField] private float _hydration;
    [SerializeField] private float _hydrationLowThreshold = 20;
    [SerializeField] private float _maxHydration = 100;
    
    [Header("Magic")]
    //[SerializeField] private float _magicLevel; // possibly shows as a number every time kp passes max slider amount, or could signify level up passsive buffs
    [SerializeField] private float _knowledgePoints;
    [SerializeField] private float _maxKnowledge = 100;
    [SerializeField] public List<float> LevelCaps = new List<float>();

    public float Mitigation = 1.0f;

    public Action OnStatsChanged = delegate { };

    #if UNITY_EDITOR
    private void OnValidate()
    {
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        _hunger = Mathf.Clamp(_hunger, 0, _maxHunger);
        _hydration = Mathf.Clamp(_hydration, 0, _maxHydration);
        _knowledgePoints = Mathf.Clamp(_knowledgePoints, 0, _maxKnowledge);
        OnStatsChanged?.Invoke();
    }
    #endif

    public float GetStat(SurvivalStatEnum stat)
    {
        return stat switch
        {
            SurvivalStatEnum.Health => _health,
            SurvivalStatEnum.Hunger => _hunger,
            SurvivalStatEnum.Hydration => _hydration,
            SurvivalStatEnum.MagicPoints => _knowledgePoints,
            _ => 0
        };
    }
    
    public float GetStatMax(SurvivalStatEnum stat)
    {
        return stat switch
        {
            SurvivalStatEnum.Health => _maxHealth,
            SurvivalStatEnum.Hunger => _maxHunger,
            SurvivalStatEnum.Hydration => _maxHydration,
            SurvivalStatEnum.MagicPoints => _maxKnowledge,
            _ => 0
        };
    }

    public bool AnyStatDead() => _health <= 0 || _hunger <= 0 || _hydration <= 0;
    public bool IsStatDead(SurvivalStatEnum stat)
    {
        return stat switch
        {
            SurvivalStatEnum.Health => _health <= 0,
            SurvivalStatEnum.Hunger => _hunger <= 0,
            SurvivalStatEnum.Hydration => _hydration <= 0,
            _ => false
        };
    }

    public bool IsStatLow(SurvivalStatEnum stat)
    {
        return stat switch
        {
            SurvivalStatEnum.Health => _health < _healthLowThreshold,
            SurvivalStatEnum.Hunger => _hunger < _hungerLowThreshold,
            SurvivalStatEnum.Hydration => _hydration < _hydrationLowThreshold,
            _ => false
        };
    }

    private void SetStat(SurvivalStatEnum stat, float value)
    {
        value = Mathf.Clamp(value, 0, GetStatMax(stat));
        switch (stat)
        {
            case SurvivalStatEnum.Health:
                _health = value;
                break;
            case SurvivalStatEnum.Hunger:
                _hunger = value;
                break;
            case SurvivalStatEnum.Hydration:
                _hydration = value;
                break;
            case SurvivalStatEnum.MagicPoints:
                _knowledgePoints = value;
                break;
        }
    }

    public void SetAllMax()
    {
        _health = _maxHealth;
        _hunger = _maxHunger;
        _hydration = _maxHydration;
        _knowledgePoints = 0;
        OnStatsChanged?.Invoke();
    }

    public void Increase(SurvivalStatEnum stat, float amount)
    {
        SetStat(stat, GetStat(stat) + amount); // could potentially add a Buff variable?
        OnStatsChanged?.Invoke();
    }
    public void Decrease(SurvivalStatEnum stat, float amount)
    {
        SetStat(stat, GetStat(stat) - amount * Mitigation);
        OnStatsChanged?.Invoke();
    }
}