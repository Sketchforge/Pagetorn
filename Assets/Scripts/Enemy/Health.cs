using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public float health = 100;

    //private float MAX_HEALTH = 100;

    public void SetHealth(float _health)
    {
        //this.MAX_HEALTH = maxHealth;
        this.health = _health;
    }

    public void Damage(int amount)
    {
        health += -amount;
    }
}
