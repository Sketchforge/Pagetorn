using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeAlphaCrawler : EnemyBase
{
    [SerializeField] private bool _logState;
    [SerializeField, ReadOnly] private CrawlerState _crawlerState;
    public CrawlerState State => _crawlerState;
    //private bool Roaming => State == CrawlerState.Roaming;
    //private bool Chasing => State == CrawlerState.Chasing;
    //private bool Attacking => State == CrawlerState.Attacking;

    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private float _attackTime;
    private float _memoryTime;

    protected override void Start()
    {
        base.Start();
        CheckState();
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        Debug.Log("Destination: " + _agent.destination);
    }

    protected override void Update()
    {
        base.Update();
        if (!_active) return;

        switch (State)
        {
        default:
        case CrawlerState.Roaming:
            Debug.Log(_crawlerState);
            _memoryTime = Time.time;
            MoveTo(roamPosition);
            if (Vector3.Distance(transform.position, roamPosition) < 50f)
            {
                //reached roam pos
                roamPosition = GetRoamingPosition();
            }

            if (CheckTarget()) TrySetState(CrawlerState.Chasing);
            break;
        case CrawlerState.Chasing:
            Debug.Log(_crawlerState);
                if (_target)
                {
                    roamPosition = GetRoamingPosition();
                    FaceTarget();


                    MoveTo(_target.position);

                    if ((Time.time - _memoryTime) > _memoryTimeout)
                        if (!CheckTarget()) TrySetState(CrawlerState.Roaming);

                    _attackTime = Time.time;

                    if (CheckAttackTarget()) TrySetState(CrawlerState.Attacking);
                }
            break;
        case CrawlerState.Attacking:
            
            //Do attack
            if (_target == PlayerManager.Instance.Player)
            {
                if ((Time.time - _attackTime) > _rateOfAttack)
                {
                    PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, _attackDamage); //switch w/ hitbox and animation later
                    TrySetState(CrawlerState.Chasing);
                    Debug.Log(_crawlerState);
                }
            }
            if (_target.tag == "AlphaCrawler")
            {
                if ((Time.time - _attackTime) > _rateOfAttack)
                {
                    _target.GetComponent<Health>().Damage(Mathf.CeilToInt(Random.Range(_attackDamage / 2, _attackDamage)));
                    TrySetState(CrawlerState.Chasing);
                    Debug.Log(_crawlerState);
                }
            }

            if (_target.tag == "Gloop") //TODO: Add !Raiding bool
            {
                TrySetState(CrawlerState.Eating);
            }
            if ((Time.time - _attackTime) > _rateOfAttack)
                TrySetState(CrawlerState.Chasing);
            break;
        case CrawlerState.Eating:
            Debug.Log(_crawlerState);
            Destroy(_target.gameObject);
            //Eat gloob animation
            //Destroy gloob
            _numberGloopsEaten++;
            TrySetState(CrawlerState.Roaming);
            break;

        }

    }

    public void CheckState()
    {
        
        if (seesTarget) //If Crawler sees a target, whether Player, Ranged, or Glob, it chases it. Once it reaches it, it will either attack Players/Ranged, or Eat globs.
        {
            TrySetState(CrawlerState.Chasing);
            if (atTarget)
            {
                if (_target == PlayerManager.Instance.Player)
                {
                    TrySetState(CrawlerState.Attacking);
                }
                else if (_target.tag == "Gloop")
                {
                    TrySetState(CrawlerState.Eating);
                }
                //TODO Add behavior for if target is Witch, to RUN!
            }
        }
        else
        {
            TrySetState(CrawlerState.Roaming);
        }
    }

    private bool TrySetState(CrawlerState newState)
    {
        if (newState == State) return false;
        if (_logState) Debug.Log($"Player State switched to {newState}", gameObject);
        _crawlerState = newState;
        return true;
    }
    
    private Vector3 GetRoamingPosition()
    {
        return startingPosition + new Vector3(Random.Range(-1f,1f), 0, Random.Range(-1,1f)).normalized * Random.Range(10f, 70f);
    }

    

}
