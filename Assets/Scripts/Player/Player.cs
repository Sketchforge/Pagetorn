using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, DrawSO] private SurvivalStat Health;
    [SerializeField, DrawSO] private SurvivalStat Hunger;
    [SerializeField, DrawSO] private SurvivalStat Hydration;
}