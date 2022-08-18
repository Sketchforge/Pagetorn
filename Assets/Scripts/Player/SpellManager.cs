using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    private Magic _data;
    private float _countDown = -100;


    // Update is called once per frame
    void Update()
    {
        Debug.Log("Spell Manager is active");

        if (_data.IsTimed == true)
            Timer(_data.Duration);
    }
    
    // OnCast is essentially start function
    public void OnCast(Magic data)
    {
        _data = data;
        if (data.CanHeal)
        {
            HealPlayer(data.Heal);
        }
        if (data.CanDamage)
        {
            DamageEnemy(data.Damage);
        }
        if (data.CanMitigate && data.IsTimed) // BUG: can cast this same spell twice, and mitigation stays between each session
        {
            Mitigate(data.Mitigation);
        }
    }

    public void HealPlayer(float amount)
    {
        PlayerManager.Instance.Survival.Increase(SurvivalStatEnum.Health, amount);
        Destroy(gameObject);
    }
    public void DamageEnemy(float baseDamage)
    {
        // TODO: figure out projectile vs instant and damage and AOE and tracking and knockback and
        PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, baseDamage); // currently attacks player lol
    }
    public void Mitigate(float mitigation)
    {
        Debug.Log("START Mitigation number is " + PlayerManager.Instance.Survival.Mitigation);
        PlayerManager.Instance.Survival.Mitigation = (100 - mitigation) / 100; // has to be timed, and lets inspector set percent
    }
    public void Timer(float duration)
    {
        if (_countDown == -100)
        {
            _countDown = duration;
            Debug.Log("time active!");
            Debug.Log("New Mitigation number is " + PlayerManager.Instance.Survival.Mitigation);
        }
        else if (_countDown > 0)
        {
            _countDown -= Time.deltaTime;
            DisplayTime(_countDown);
        }
        else
        {
            Debug.Log("Time up!");
            _countDown = -100;
            if (_data.CanMitigate) PlayerManager.Instance.Survival.Mitigation = 1.0f;
            Debug.Log("Mitigation number is " + PlayerManager.Instance.Survival.Mitigation);
            Destroy(gameObject);
        }
    }

    public void DisplayTime(float displayTime)
    {
        float minutes = Mathf.FloorToInt(displayTime / 60);
        float seconds = Mathf.FloorToInt(displayTime % 60);
        Debug.Log(minutes + ":" + seconds);
    }
}
