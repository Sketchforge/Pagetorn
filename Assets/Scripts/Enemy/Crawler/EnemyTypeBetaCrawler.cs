using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTypeBetaCrawler : EnemyBase
{
    [SerializeField] private bool _logState;
    [SerializeField, ReadOnly] private CrawlerState _crawlerState;
    public CrawlerState State => _crawlerState;

    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private Vector3 followPosition;
    private float _attackTime;
    private float _memoryTime;

    [SerializeField] private Transform myAlpha = null;

    protected override void Start()
    {
        base.Start();
        //CheckState();
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        followPosition = GetFollowPosition();
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
                if (myAlpha && Vector3.Distance(transform.position, myAlpha.position) > 50f)
                {
                    TrySetState(CrawlerState.Following);
                    break;
                }
                if (!myAlpha && Vector3.Distance(transform.position, roamPosition) < 50f)
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
                    
                    FaceTarget();
                    //followPosition = GetRoamingPosition();

                    if (_target.tag == "Witch")
                    {
                        TrySetState(CrawlerState.Fleeing);
                        break;
                    }
                    if (_target.tag == "AlphaCrawler")
                    {
                        if (myAlpha == null)
                        {
                            Debug.Log("Found a new dad!");
                            myAlpha = _target;
                            TrySetState(CrawlerState.Following);
                            break;
                        }
                        if (!myAlpha.gameObject.activeSelf)
                        {
                            myAlpha = null;
                        }
                    }

                    MoveTo(_target.position);

                    if ((Time.time - _memoryTime) > _memoryTimeout)
                        if (!CheckTarget()) TrySetState(CrawlerState.Following);

                    _attackTime = Time.time;

                    if (CheckAttackTarget()) TrySetState(CrawlerState.Attacking);
                }
                break;

            case CrawlerState.Following: //follow the Alpha
                Debug.Log("Following Daddy...");
                _memoryTime = Time.time;
                MoveTo(followPosition);
                
                if (Vector3.Distance(transform.position, followPosition) < 10f)
                {
                    //reached roam pos
                    followPosition = GetFollowPosition();
                }

                if (CheckTarget()) TrySetState(CrawlerState.Chasing);
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
                if (_target.tag == "AlphaCrawler" && _target != myAlpha)
                {
                    if ((Time.time - _attackTime) > _rateOfAttack)
                    {
                        _target.GetComponent<Health>().Damage(Mathf.CeilToInt(Random.Range(_attackDamage / 2, _attackDamage)));
                        TrySetState(CrawlerState.Chasing);
                        Debug.Log(_crawlerState);
                    }
                }
                if (_target.tag == "Gloop" && myAlpha == null) //TODO: Add !Raiding bool
                {
                    TrySetState(CrawlerState.Eating);
                }
                if ((Time.time - _attackTime) > _rateOfAttack)
                    TrySetState(CrawlerState.Chasing);
                break;

            case CrawlerState.Eating:
                Debug.Log(_crawlerState);
                //Eat gloob animation
                Destroy(_target.gameObject);
                _numberGloopsEaten++;
                if (_numberGloopsEaten >= 1 && !myAlpha && nextStage)
                {
                    GameObject instance = Instantiate(nextStage, this.transform.position, Quaternion.identity);
                    Destroy(this.gameObject);
                    break;
                }
                TrySetState(CrawlerState.Following);
                break;

            case CrawlerState.Fleeing:
                Debug.Log(_crawlerState);

                float distance = Vector3.Distance(transform.position, _target.transform.position);

                if (distance < distanceToRun)
                {
                    Vector3 dirToTarget = transform.position - _target.transform.position;

                    Vector3 newPos = transform.position + dirToTarget;

                    MoveTo(newPos);
                }
                else if (distance >= distanceToRun)
                    TrySetState(CrawlerState.Following);
                break;

        }

    }

    private bool TrySetState(CrawlerState newState)
    {
        if (newState == State) return false;
        if (_logState) Debug.Log($"Player State switched to {newState}", gameObject);
        _crawlerState = newState;
        return true;
    }

    private Vector3 GetFollowPosition()
    {
        return startingPosition + myAlpha.position * Random.Range(2f, 5f);
    }
    private Vector3 GetRoamingPosition()
    {
        return startingPosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1, 1f)).normalized * Random.Range(10f, 70f);
    }


}