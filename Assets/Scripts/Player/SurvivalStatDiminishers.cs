using UnityEngine;

public class SurvivalStatDiminishers : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        //PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, 0.01f);
        PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Hunger, 0.5f * Time.fixedDeltaTime);
        PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Hydration, 0.7f * Time.fixedDeltaTime);
    }
}
