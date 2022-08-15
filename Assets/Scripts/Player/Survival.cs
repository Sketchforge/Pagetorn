using System;
using System.Collections;
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
    //[SerializeField] private float _magicLevel = 0; // you know, just in case :)
    [SerializeField] private float _knowledgePoints = 0;

    public Action OnStatsChanged = delegate { };

    #if UNITY_EDITOR
    private void OnValidate()
    {
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        _hunger = Mathf.Clamp(_hunger, 0, _maxHunger);
        _hydration = Mathf.Clamp(_hydration, 0, _maxHydration);
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
        }
    }

    public void SetAllMax()
    {
        _health = _maxHealth;
        _hunger = _maxHunger;
        _hydration = _maxHydration;
        OnStatsChanged?.Invoke();
    }

    public void Increase(SurvivalStatEnum stat, float amount)
    {
        SetStat(stat, GetStat(stat) + amount);
        OnStatsChanged?.Invoke();
    }
    public void Decrease(SurvivalStatEnum stat, float amount) => Increase(stat, -amount);
}