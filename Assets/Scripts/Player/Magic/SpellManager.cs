using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellManager : MonoBehaviour
{
    private Magic _data;
    private float _countDown = -100;
    public Text timeText;

    private SphereCollider spellCollider;
    private Rigidbody spellRB;
    private DataReactor _theDataReactor;

    // OnCast is essentially start function
    public void OnCast(Magic data)
    {
        _data = data;
        spellCollider = GetComponent<SphereCollider>();
        spellCollider.radius = data.ActiveRadius;
        spellRB = GetComponent<Rigidbody>();
        spellRB.isKinematic = true;
        //PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.MagicPoints, _data._knowledgePoints/ _data._pageAmount);

        if (data.CanHeal)
        {
            HealPlayer(data.Heal);
        }
        if (data.CanMitigate && data.IsTimed) // BUG: spell can be cast twice, and mitigation saves between each session
        {
            Mitigate(data.Mitigation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Spell Manager is active");

        if (_data.IsTimed == true)
            Timer(_data.Duration);
        if (_data.IsProjectile && _data.ProjectileSpeed > 0)
            Projectile(_data.ProjectileSpeed);
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
    public void Projectile(float speed) // BUG: Continues even after game is paused
    {
        if (speed > 0) transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Object.Destroy(gameObject, 100/speed); // BUG: Will conflict with timer/duration, as the object can be destroyed when either hits their limit
    }

    public void Mitigate(float mitigation)
    {
        Debug.Log("START Mitigation number is " + PlayerManager.Instance.Survival.Mitigation);
        PlayerManager.Instance.Survival.Mitigation = (100 - mitigation) / 100; // should need to be timed, but lets inspector set percent
    }
    public void Timer(float duration) // BUG: Timer counts down even when paused
    {
        if (_countDown == -100)
        {
            _countDown = duration;
            Debug.Log("time active!");
            Debug.Log("New Mitigation number is " + PlayerManager.Instance.Survival.Mitigation);
        }
        else if (_countDown > 0)
        {
            DisplayTime(_countDown);
            _countDown -= Time.deltaTime;
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

    public void DisplayTime(float displayTime) // TODO: Connect to in game timer, either in a menu or otherwise
    {
        float minutes = Mathf.FloorToInt(displayTime / 60);
        float seconds = Mathf.FloorToInt(displayTime % 60);
        //timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        //Debug.Log(timeText);
    }

    private void OnTriggerEnter(Collider other) // TODO: should objects deal damage here, hone in from range, or use the same range/attack spheres as the AI?
    {
        Debug.Log("Spell hit something");
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            Targetable _targetable = enemy.GetComponent<Targetable>();
            if (_targetable.Type != TargetableType.Witch)
            {
                if (other.GetComponent<EnemyBase>())
                {
                    other.GetComponent<EnemyBase>().GetComponent<Health>().Damage(_data.Damage);
                    Destroy(gameObject);
                }
                else if (_targetable.Type == TargetableType.Witch)
                {
                    if (!_theDataReactor) _theDataReactor = FindObjectOfType<DataReactor>();

                    else if (_theDataReactor) _theDataReactor.ForceTeleportLibrarian(true);

                    Destroy(gameObject);
                }
            }
        }

        if (other.gameObject.layer == 0)
        {
            Destroy(gameObject);
        }
        //if (_data.CanDamage && other.CompareTag("enemy")) DamageEnemy(_data.Damage);
       
    }
}
