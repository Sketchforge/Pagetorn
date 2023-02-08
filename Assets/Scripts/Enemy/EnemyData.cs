using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Pagetorn/Enemy", order =1)]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float _maxHealth = 1;
    [SerializeField] private float _attackDamage = 1;
    [SerializeField] private float _moveSpeed = 1;
    [SerializeField] private float _rateOfAttack = 4;
    [SerializeField] private float _rangeOfVision = 50;
    [SerializeField] private float _attackRange = 12;
    [SerializeField] private float _memoryTimeout = 5;
    [SerializeField] private List<GameObject> _loot;

    public float MaxHealth => _maxHealth;
    public float AttackDamage => _attackDamage;
    public float MoveSpeed => _moveSpeed;
    public float RateOfAttack => _rateOfAttack;
    public float RangeOfVision => _rangeOfVision;
    public float AttackRange => _attackRange;
    public float MemoryTimeout => _memoryTimeout;
    public List<GameObject> Loot => _loot;
}
