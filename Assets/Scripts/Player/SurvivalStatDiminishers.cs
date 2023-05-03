using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalStatDiminishers : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

            PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Hunger, 0.00000001f);
            PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Hydration, 0.00001f);
    }
}
