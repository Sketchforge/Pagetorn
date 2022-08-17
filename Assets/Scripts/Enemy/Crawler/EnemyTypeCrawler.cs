using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeCrawler : EnemyBase
{
    [Header("Crawler Specific")]
    [SerializeField] private EnemyData _alphaData;
    [SerializeField] private bool _isAlpha;
    [SerializeField, ReadOnly] private CrawlerState _crawlerState;

    [SerializeField] private Targetable _targetable;
    [SerializeField] private GameObject _crawlerArt;
    [SerializeField] private GameObject _alphaArt;

    [SerializeField] private EnemyTypeCrawler _alpha;
    [SerializeField] private List<EnemyTypeCrawler> _children = new List<EnemyTypeCrawler>();
    [SerializeField] private Vector2 _randomRoamRange = new Vector2(10f, 50f);
    [SerializeField] private Vector2 _randomFollowRange = new Vector2(2f, 5f);
    
    [Header("Debug")]
    [SerializeField, ReadOnly] private Vector3 _startingPosition;
    [SerializeField, ReadOnly] private Vector3 _roamPosition;
    [SerializeField, ReadOnly] private float _attackTime;
    [SerializeField, ReadOnly] private float _memoryTime;
    [SerializeField, ReadOnly] private float _roamTime;
    [SerializeField] private bool hasRoamPos = true;

    public override EnemyData Data => _isAlpha ? _alphaData : base.Data;

    public bool HasAlpha => _alpha != null;

    protected override void OnStart()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        UpdateAlphaStatus();
    }

    protected override void OnUpdate()
    {
        if (HasAlpha && Vector3.Distance(transform.position, _alpha.transform.position) > 55f)
        {
            _alpha = null;
        }
        if (_target && Vector3.Distance(transform.position, _target.transform.position) > 55f)
        {
            _target = null;
        }
        
        switch (_crawlerState)
        {
            default:
            case CrawlerState.Roaming:
                OnRoamingState();
                break;
            case CrawlerState.Chasing:
                OnChasingState();
                break;
            case CrawlerState.Attacking:
                OnAttackingState();
                break;
            case CrawlerState.Eating:
                OnEatingState();
                break;
            case CrawlerState.Fleeing:
                OnFleeingState();
                break;
        }
    }

    protected override void OnLoseTarget()
    {
        _crawlerState = CrawlerState.Roaming;
    }

    [Button]
    private void UpdateAlphaStatus()
    {
        Log("Update Alpha Status: " + _isAlpha);
        _targetable.SetType(_isAlpha ? TargetableType.AlphaCrawler : TargetableType.Crawler);
        _crawlerArt.SetActive(!_isAlpha);
        _alphaArt.SetActive(_isAlpha);
    }

    #region State Machine

    private void OnRoamingState()
    {
        _memoryTime = Time.time;
        if (Vector3.Distance(transform.position, _roamPosition) > 20f)
        {
            _roamPosition = GetRoamingPosition();
        }
        FacePosition(_roamPosition);
        MoveTo(_roamPosition);
        if (Vector3.Distance(transform.position, _roamPosition) < 0.5f || (HasAlpha && Vector3.Distance(transform.position, _alpha.transform.position) > 10f))
        {
            Debug.Log("RoamStateIsBeingGlitchy");
            _roamPosition = GetRoamingPosition();
        }

        if (CheckTarget()) TrySetState(CrawlerState.Chasing);
    }
    
    private void OnChasingState()
    {
        if (!_target)
        {
            TrySetState(CrawlerState.Roaming);
            return;
        }
        FaceTarget();

        if (_target.Type == TargetableType.Witch)
        {
            TrySetState(CrawlerState.Fleeing);
            return;
        }

        MoveTo(_target.transform.position);

        if ((Time.time - _memoryTime) > Data.MemoryTimeout)
        {
            if (!CheckTarget())
            {
                TrySetState(CrawlerState.Roaming);
                return;
            }
        }

        _attackTime = Time.time;

        if (CheckAttackTarget()) TrySetState(CrawlerState.Attacking);
    }
    
    private void OnAttackingState()
    {
        if (!_target)
        {
            TrySetState(CrawlerState.Roaming);
            return;
        }
        FaceTarget();
        if (_target.Type == TargetableType.Player)
        {
            if ((Time.time - _attackTime) > Data.RateOfAttack)
            {
                PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, Data.AttackDamage); //switch w/ hitbox and animation later 
                TrySetState(CrawlerState.Chasing);
            }
        }
        if (_target.Type == TargetableType.AlphaCrawler)
        {
            if ((Time.time - _attackTime) > Data.RateOfAttack)
            {
                _target.GetComponent<Health>().Damage(Mathf.CeilToInt(Random.Range(Data.AttackDamage / 2, Data.AttackDamage)));
                TrySetState(CrawlerState.Chasing);
            }
        }
        if (_target.Type == TargetableType.Gloop && !HasAlpha) //TODO: Add !Raiding bool 
        {
            TrySetState(CrawlerState.Eating);
        }
        if ((Time.time - _attackTime) > Data.RateOfAttack)
            TrySetState(CrawlerState.Chasing);
    }

    private void OnEatingState()
    {
        if (!_target || _target.Type != TargetableType.Gloop || !_isAlpha && HasAlpha)
        {
            return;
        }
        var gloop = _target.GetComponent<Gloop>();
        if (gloop == null || gloop.Eaten)
        {
            TrySetState(CrawlerState.Roaming);
            return;
        }
        gloop.Eaten = true;
        Log("Is Eating: " + _target.name);
        Destroy(_target.gameObject);
        _numberGloopsEaten++;
        if (_numberGloopsEaten >= 1 && !HasAlpha)
        {
            _isAlpha = true;
            UpdateAlphaStatus();
        }
        TrySetState(CrawlerState.Roaming);
    }
    
    private void OnFleeingState()
    {
        float distance = Vector3.Distance(transform.position, _target.transform.position);

        if (distance < distanceToRun)
        {
            Vector3 dirToTarget = transform.position - _target.transform.position;

            Vector3 newPos = transform.position + dirToTarget;

            MoveTo(newPos);
        }
        else if (distance >= distanceToRun)
        {
            TrySetState(CrawlerState.Roaming);
        }
    }
    
    #endregion

    // ReSharper disable Unity.PerformanceAnalysis
    private bool TrySetState(CrawlerState newState, bool fromAlpha = false)
    {
        if (HasAlpha && !fromAlpha)
        {
            // Brute force to fix bugs
            _crawlerState = _alpha._crawlerState;
            return false;
        }
        if (newState == _crawlerState) return false;
        Log($"State switched to {newState}");
        _crawlerState = newState;
        if (_isAlpha)
        {
            var childState = _crawlerState;
            bool alphaIsEating = childState == CrawlerState.Eating;
            if (alphaIsEating) childState = CrawlerState.Roaming;
            foreach (var crawler in _children)
            {
                crawler.TrySetState(childState, true);
                if (!alphaIsEating) crawler._target = _target;
            }
        }
        _startingPosition = transform.position;
        return true;
    }
    
    private Vector3 GetRoamingPosition()
    {
        if (Time.time - _roamTime < 1f && !hasRoamPos) return _roamPosition;
        _roamTime = Time.time;
        var rand = Random.insideUnitSphere * GetRandom(HasAlpha ? _randomFollowRange : _randomRoamRange);
        rand.y = 0;
        if (HasAlpha)
        {
            hasRoamPos = false;
            return rand + _alpha.transform.position + -_alpha.transform.forward * Random.Range(0.5f, 2.5f);
        }
        else
        {
            hasRoamPos = false;
            return rand + _startingPosition;
        }
    }

    private static float GetRandom(Vector2 range) => Random.Range(range.x, range.y);

    protected override Targetable GetPotentialTarget(IEnumerable<Targetable> targets)
    {
        if (HasAlpha) return null;
        Targetable target = null;
        int targetPriority = -1;
        foreach (Targetable t in targets)
        {
            if (targetPriority < 10 && t.Type == TargetableType.Witch)
            {
                target = t; //target to run FROM librarian
                targetPriority = 10;
            }
            if (targetPriority < 5 && t.Type == TargetableType.Player)
            {
                target = t;
                targetPriority = 5;
            }
            if (t.Type == TargetableType.AlphaCrawler && t.transform != transform)
            {
                if (!_isAlpha)
                {
                    _alpha = t.GetComponent<EnemyTypeCrawler>();
                    if (HasAlpha) _alpha.AddFollower(this);
                }
                if (targetPriority < 1)
                {
                    target = t;
                    targetPriority = 1;
                }
            }
            if (targetPriority < 0 && t.Type == TargetableType.Gloop)
            {
                target = t;
                targetPriority = 0;
            }
        }
        return target;
    }

    private void AddFollower(EnemyTypeCrawler crawler)
    {
        if (!_isAlpha) return;
        _children.Add(crawler);
    }
}