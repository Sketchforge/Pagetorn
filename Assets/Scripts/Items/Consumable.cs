using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Item/Consumable")]
public class Consumable : Item
{
    [Header("TypeOfConsumable")]
    [SerializeField] bool affectsHealth;
    [SerializeField] bool affectsHunger;
    [SerializeField] bool affectsHydration;

    [SerializeField] float healthChange = 0;
    [SerializeField] float hungerChange = 0;
    [SerializeField] float hydrationChange = 0;

    public void UseItem(InventoryItemSlot slot)
    {
        if (affectsHunger)
        {
            PlayerManager.Instance.Survival.Increase(SurvivalStatEnum.Hunger, hungerChange);
        }
        if (affectsHealth)
        {
            PlayerManager.Instance.Survival.Increase(SurvivalStatEnum.Health, healthChange);
        }
        if (affectsHydration)
        {
            PlayerManager.Instance.Survival.Increase(SurvivalStatEnum.Hydration, hydrationChange);
        }

        //slot.DamageItem(1);
        slot.RemoveItem(1);
    }
}
