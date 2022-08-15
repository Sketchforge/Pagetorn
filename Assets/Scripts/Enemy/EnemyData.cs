using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Pagetorn/Enemy", order =1)]
public class EnemyData : ScriptableObject
{
    public float HP;
    public float damage;
    public float moveSpeed;
    public float attackRate;
    public float rangeOfVision;
}
