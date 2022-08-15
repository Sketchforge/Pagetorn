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
                MoveTo(roamPosition);
                if (Vector3.Distance(transform.position, roamPosition) < 1f)
                {
                    //reached roam pos
                    roamPosition = GetRoamingPosition();
                }
                if (FindPlayer()) TrySetState(CrawlerState.Chasing);
                break;
            case CrawlerState.Chasing:
                Debug.Log(_crawlerState);
                FaceTarget();
                roamPosition = GetRoamingPosition();
                MoveTo(_playerTarget.position);
                if (!FindPlayer()) TrySetState(CrawlerState.Roaming);
                if (CheckAttackTarget()) TrySetState(CrawlerState.Attacking);
                break;
            case CrawlerState.Attacking:
                Debug.Log(_crawlerState);
                //Do attack
                if (_playerTarget.tag == "Player")
                {
                    //Deal damage
                }
                TrySetState(CrawlerState.Chasing);
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
                if (_playerTarget == PlayerManager.Instance.Player)
                {
                    TrySetState(CrawlerState.Attacking);
                }
                else if (_playerTarget.tag == "Glob")
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
