using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalStatDiminishers : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        //PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, 0.01f);
        PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Hunger, 0.001f);
        PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Hydration, 0.005f);
    }
}
