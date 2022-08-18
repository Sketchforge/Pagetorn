using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTypeFlyer : EnemyBase
{
    [Header("Flyer Specific")]
    [SerializeField] private EnemyData _alphaData;
    [SerializeField] private bool _isAlpha;
    [SerializeField, ReadOnly] private FlyerState _flyerState;
    [SerializeField] private Vector3 _circleOffset = new Vector3(0, 5, 10);
    [SerializeField] protected float _circleTimeout = 10;
    [SerializeField] private bool isCircling = false;
    [SerializeField] public int _flyingHeight = 10;

    [SerializeField] private Targetable _targetable;
    [SerializeField] private GameObject _flyerArt;
    [SerializeField] private GameObject _alphaArt;

    [SerializeField] private EnemyTypeFlyer _alpha;
    [SerializeField] private List<EnemyTypeFlyer> _children = new List<EnemyTypeFlyer>();
    [SerializeField] private Vector2 _randomRoamRange = new Vector2(10f, 50f);
    [SerializeField] private Vector2 _randomFollowRange = new Vector2(2f, 5f);


    [Header("Debug")]
    [SerializeField, ReadOnly] private Vector3 _startingPosition;
    [SerializeField, ReadOnly] private Vector3 _roamPosition;
    [SerializeField, ReadOnly] private float _attackTime;
    [SerializeField, ReadOnly] private float _memoryTime;
    [SerializeField, ReadOnly] private float _roamTime;
    [SerializeField, ReadOnly] private float _circlingTime;
    [SerializeField] private bool hasRoamPos = true;
    [SerializeField] private bool hasCircPos = true;
    [SerializeField, ReadOnly] private float originalYPos;
    // //Circle Settings:
    // [SerializeField, ReadOnly] float xCenter;
    // [SerializeField, ReadOnly] float yCenter;
    // [SerializeField, ReadOnly] float zCenter; //center of rotation circle
    // [SerializeField, ReadOnly] float radius; //radius for circle
    // [SerializeField, ReadOnly] float angleH; //angle to use for rotation
    // [SerializeField, ReadOnly] float angleV;
    // [SerializeField, ReadOnly] float degH; //timer to increment to init rotation
    // [SerializeField, ReadOnly] float degV;

    public override EnemyData Data => _isAlpha ? _alphaData : base.Data;

    public bool HasAlpha => _alpha != null;

    protected override void OnStart()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        originalYPos = transform.position.y; //NOT permanent
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

        switch (_flyerState)
        {
            default:
            case FlyerState.Roaming:
                OnRoamingState();
                break;
            case FlyerState.Chasing:
                OnChasingState();
                break;
            case FlyerState.Circling:
                OnCirclingState();
                break;
            case FlyerState.Attacking:
                OnAttackingState();
                break;
            case FlyerState.Gathering:
                //OnGatheringState();
                break;
            case FlyerState.Eating:
                OnEatingState();
                break;
            case FlyerState.Nesting:
                //OnNestingState();
                break;
            case FlyerState.Fleeing:
                OnFleeingState();
                break;
        }
    }

    protected override void OnLoseTarget()
    {
        _flyerState = FlyerState.Roaming;
    }

    [Button]
    private void UpdateAlphaStatus()
    {
        Log("Update Alpha Status: " + _isAlpha);
        _targetable.SetType(_isAlpha ? TargetableType.AlphaFlyer : TargetableType.Flyer);
        _flyerArt.SetActive(!_isAlpha);
        _alphaArt.SetActive(_isAlpha);
    }

    #region State Machine

    private void OnRoamingState()
    {
        _memoryTime = Time.time;
        GameObject artToControl = _isAlpha ? _alphaArt : _flyerArt;
        artToControl.transform.position = new Vector3(transform.position.x, originalYPos + _flyingHeight, transform.position.z);

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
        if (CheckTarget()) TrySetState(FlyerState.Chasing);
    }

    private void OnChasingState()
    {
        if (!_target)
        {
            TrySetState(FlyerState.Roaming);
            return;
        }
        FaceTarget();

        if (_target.Type == TargetableType.Witch)
        {
            TrySetState(FlyerState.Fleeing);
            return;
        }

        MoveTo(_target.transform.position);

        if ((Time.time - _memoryTime) > Data.MemoryTimeout)
        {
            if (!CheckTarget())
            {
                TrySetState(FlyerState.Roaming);
                return;
            }
        }

        _circlingTime = Time.time;
        if (CheckCircleTarget()) TrySetState(FlyerState.Circling);
        //_attackTime = Time.time;
        //if (CheckAttackTarget()) TrySetState(FlyerState.Attacking);
    }

    private void OnCirclingState()
    {
        if (!_target)
        {
            TrySetState(FlyerState.Roaming);
            return;
        }
        if (_target)
        {
            MoveTo(CircleTarget(_target));
            GameObject artToControl = _isAlpha ? _alphaArt : _flyerArt;
            
            artToControl.transform.position = new Vector3(transform.position.x, transform.position.y + _flyingHeight, transform.position.z);
            if ((Time.time - _circlingTime) > _circleTimeout)
            {
                //TODO: Swoop down, attack, swoop back up.
                _attackTime = Time.time;
                isCircling = false;
                artToControl.transform.position = new Vector3(transform.position.x, originalYPos, transform.position.z);
                if (CheckAttackTarget()) TrySetState(FlyerState.Attacking);
            }
        }
    }

    private void OnAttackingState()
    {
        if (!_target)
        {
            TrySetState(FlyerState.Roaming);
            return;
        }
        FaceTarget();
        if (_target.Type == TargetableType.Player)
        {
            if ((Time.time - _attackTime) > Data.RateOfAttack)
            {
                PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, Data.AttackDamage); //switch w/ hitbox and animation later 
                TrySetState(FlyerState.Chasing);
            }
        }
        if (_target.Type == TargetableType.AlphaFlyer)
        {
            if ((Time.time - _attackTime) > Data.RateOfAttack)
            {
                _target.GetComponent<Health>().Damage(Mathf.CeilToInt(Random.Range(Data.AttackDamage / 2, Data.AttackDamage)));
                TrySetState(FlyerState.Chasing);
            }
        }
        if (_target.Type == TargetableType.Gloop && !HasAlpha) //TODO: Add !Raiding bool 
        {
            TrySetState(FlyerState.Eating);
        }
        if ((Time.time - _attackTime) > Data.RateOfAttack)
            TrySetState(FlyerState.Chasing);
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
            TrySetState(FlyerState.Roaming);
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
        TrySetState(FlyerState.Roaming);
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
            TrySetState(FlyerState.Roaming);
        }
    }

    #endregion

    // ReSharper disable Unity.PerformanceAnalysis
    private bool TrySetState(FlyerState newState, bool fromAlpha = false)
    {
        if (HasAlpha && !fromAlpha)
        {
            // Brute force to fix bugs
            _flyerState = _alpha._flyerState;
            return false;
        }
        if (newState == _flyerState) return false;
        Log($"State switched to {newState}");
        _flyerState = newState;
        if (_isAlpha)
        {
            var childState = _flyerState;
            bool alphaIsEating = childState == FlyerState.Eating;
            if (alphaIsEating) childState = FlyerState.Roaming;
            foreach (var flyer in _children)
            {
                flyer.TrySetState(childState, true);
                if (!alphaIsEating) flyer._target = _target;
            }
        }
        _startingPosition = transform.position;
        return true;
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
            if (t.Type == TargetableType.AlphaFlyer && t.transform != transform)
            {
                if (!_isAlpha)
                {
                    _alpha = t.GetComponent<EnemyTypeFlyer>();
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

    private void AddFollower(EnemyTypeFlyer flyer)
    {
        if (!_isAlpha) return;
        _children.Add(flyer);
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

    private Vector3 CircleTarget(Targetable targetToCircle)
    {
        Debug.Log("Attempting to circle");

        //if (Time.time - _circlingTime < 1f && !hasCircPos) return _circPosition;
        //_circlingTime = Time.time;
        var rand = Random.insideUnitSphere * GetRandom(HasAlpha ? _randomFollowRange : _randomRoamRange);
        Vector3 _targetPos = new Vector3(_target.transform.position.x, 10, _target.transform.position.z); //TODO change back lol

        return rand + _targetPos + -_target.transform.right * Random.Range(0.5f, 2.5f);
    }


    protected bool CheckCircleTarget()
    {
        return (Vector3.Distance(transform.position, _target.transform.position) < Data.AttackRange + 20);
    }

}