using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

public class EnemyTypeRanger : EnemyBase
{
    [Header("Ranger Specific")]
    [SerializeField] private EnemyData _alphaData;
    [SerializeField] private bool _isAlpha = false;
    [SerializeField, ReadOnly] private RangerState _rangerState;

    [SerializeField] private Targetable _targetable;
    [SerializeField] private GameObject _rangerArt;
    [SerializeField] private GameObject _alphaArt;

    [SerializeField] private EnemyTypeRanger _alpha;
    [SerializeField] private List<EnemyTypeRanger> _children = new List<EnemyTypeRanger>();
    [SerializeField] private Vector2 _randomRoamRange = new Vector2(10f, 50f);
    [SerializeField] private Vector2 _randomFollowRange = new Vector2(2f, 5f);
    [SerializeField] protected float _radiusSurroundTarget = 30f;
    [SerializeField] protected float _chargeShotTime = 4f;
    [SerializeField] protected Transform _bulletOrigin;
    [SerializeField] private GameObject bulletObj;
    [SerializeField] private float bulletForce = 30;
    [SerializeField] private GameObject myBullet;

    [Header("Debug")]
    [SerializeField, ReadOnly] private Vector3 _startingPosition;
    [SerializeField, ReadOnly] private Vector3 _roamPosition;
    [SerializeField, ReadOnly] private float _attackTime;
    [SerializeField, ReadOnly] private float _chargeTime;
    [SerializeField, ReadOnly] private float _memoryTime;
    [SerializeField, ReadOnly] private float _roamTime;
    [SerializeField] private bool hasRoamPos = true;

    private bool _playedWalkingSound = false;

    public override EnemyData Data => _isAlpha ? _alphaData : base.Data;

    public bool HasAlpha => _alpha != null;

    protected override void OnAwake()
    {
        _startingPosition = transform.position;
    }

    protected override void OnStart()
    {
        
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

        //CHECKS FOR HITS OUTSIDE OF STATES
        //if (myBullet != null)
        //    if (myBullet.GetComponent<RangerBullet>().hitPlayer)
        //        PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, Random.Range(Data.AttackDamage / 2, Data.AttackDamage));

        switch (_rangerState)
        {
            default:
            case RangerState.Roaming:
                OnRoamingState();
                break;
            case RangerState.Chasing:
                OnChasingState();
                break;
            case RangerState.Charging:
                OnChargingState();
                break;
            case RangerState.Attacking:
                OnAttackingState();
                break;
            case RangerState.Eating:
                OnEatingState();
                break;
            case RangerState.Fleeing:
                OnFleeingState();
                break;
        }

    }

    protected override void OnLoseTarget()
    {
        _rangerState = RangerState.Roaming;
    }

    [Button]
    private void UpdateAlphaStatus()
    {
        Log("Update Alpha Status: " + _isAlpha);
        _targetable.SetType(_isAlpha ? TargetableType.AlphaRanger : TargetableType.Ranger); //targetable is null... why
        _rangerArt.SetActive(!_isAlpha);
        _alphaArt.SetActive(_isAlpha);
    }

    #region State Machine

    private void OnRoamingState()
    {
        //Debug.Log("RoamState");
        _memoryTime = Time.time;
        if (Vector3.Distance(transform.position, _roamPosition) > 20f)
        {
            _roamPosition = GetRoamingPosition();
        }
        FacePosition(_roamPosition);
        MoveTo(_roamPosition);
        if (!_playedWalkingSound)
        {
            _moveSound.PlayAtParentAndFollow(this.transform);
            _playedWalkingSound = true;

        }
        if (Vector3.Distance(transform.position, _roamPosition) < 0.5f || (HasAlpha && Vector3.Distance(transform.position, _alpha.transform.position) > 10f))
        {
            //Debug.Log("RoamStateIsBeingGlitchy");
            _playedWalkingSound = false;
            _roamPosition = GetRoamingPosition();
        }

        if (CheckTarget()) TrySetState(RangerState.Chasing);
    }

    private void OnChasingState()
    {
        //Debug.Log("ChaseState");
        if (!_target)
        {
            TrySetState(RangerState.Roaming);
            return;
        }
        FaceTarget();

        if (_target.Type == TargetableType.Witch)
        {
            TrySetState(RangerState.Fleeing);
            return;
        }

        MoveTo(_target.transform.position + new Vector3(_randomFollowRange.x / 2, 0, _randomFollowRange.y / 2));
        if (_isAlpha)
        {
            makeFollowersCircleTarget();
            _randomFollowRange = new Vector2(15f, 20f); //add more offset, too close to player
        }
            

        if ((Time.time - _memoryTime) > Data.MemoryTimeout)
        {
            if (!CheckTarget())
            {
                TrySetState(RangerState.Roaming);
                return;
            }
        }

        _attackTime = Time.time;
        _chargeTime = Time.time;

        if (CheckAttackTarget())
        {
            TrySetState(RangerState.Attacking);
        }
    }

    private void OnChargingState()
    {
        if (!_isAlpha)
        {
            if ((Time.time - _chargeTime) < Random.Range(_chargeShotTime - 1f, _chargeShotTime))
            {
                Debug.Log("Charging...");
            }

            if ((Time.time - _chargeTime) > Random.Range(_chargeShotTime - 1f, _chargeShotTime))
            {
                _attackTime = Time.time;
                if (CheckAttackTarget()) //check again, if still in range, shoot
                {
                    TrySetState(RangerState.Attacking);
                }
                else
                {
                    TrySetState(RangerState.Roaming);
                }
            }
        }

    }

    private void OnAttackingState()
    {
        //Debug.Log("AttackState");
        if (!_target)
        {
            TrySetState(RangerState.Roaming);
            return;
        }
        FaceTarget();
        if (_target.Type == TargetableType.Player)
        {
            //if (!_isAlpha)
            //{
                if ((Time.time - _attackTime) > Random.Range(Data.RateOfAttack - 1f, Data.RateOfAttack)) //if not an alpha, simply deal damage
                {
                    //PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, Random.Range(Data.AttackDamage / 2, Data.AttackDamage)); //switch w/ hitbox and animation later 
                    Debug.Log("Shot Glob");
                    _attackSound.PlayAtPosition(transform.position);
                    myBullet = Instantiate(bulletObj, _bulletOrigin.position, Quaternion.identity, null);
                    myBullet.GetComponent<RangerBullet>().parentData = Data;
                    myBullet.GetComponent<RangerBullet>().targetTransform = _target.transform;
                    //myBullet.GetComponent<RangerBullet>().rb.velocity = transform.forward * 10f;
                    //myBullet.GetComponent<RangerBullet>().rb.AddForce(transform.forward * bulletForce);

                    _attackTime = Time.time;
                    TrySetState(RangerState.Roaming);
                }
            //}
        }
        if (_target.Type == TargetableType.AlphaRanger)
        {
            if ((Time.time - _attackTime) > Random.Range(Data.RateOfAttack - 1f, Data.RateOfAttack))
            {
                //_target.GetComponent<Health>().Damage(Mathf.CeilToInt(Random.Range(Data.AttackDamage / 2, Data.AttackDamage)));
                Debug.Log("Shot Glob at Alpha");
                _attackTime = Time.time;
                TrySetState(RangerState.Roaming);
            }
        }
        if (_target.Type == TargetableType.Gloop && !HasAlpha) //TODO: Add !Raiding bool 
        {
            TrySetState(RangerState.Eating);
        }
        if ((Time.time - _attackTime) > Data.RateOfAttack)
            TrySetState(RangerState.Chasing);
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
            TrySetState(RangerState.Roaming);
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
        TrySetState(RangerState.Roaming);
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
            TrySetState(RangerState.Roaming);
        }
    }

    #endregion

    // ReSharper disable Unity.PerformanceAnalysis
    private bool TrySetState(RangerState newState, bool fromAlpha = false)
    {
        if (HasAlpha && !fromAlpha)
        {
            // Brute force to fix bugs
            _rangerState = _alpha._rangerState;
            return false;
        }
        if (newState == _rangerState) return false;
        Log($"State switched to {newState}");
        _rangerState = newState;
        if (_isAlpha)
        {
            var childState = _rangerState;
            bool alphaIsEating = childState == RangerState.Eating;
            if (alphaIsEating) childState = RangerState.Roaming;
            foreach (var ranger in _children)
            {
                ranger.TrySetState(childState, true);
                if (!alphaIsEating) ranger._target = _target;
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
            if (t.Type == TargetableType.AlphaRanger && t.transform != transform)
            {
                if (!_isAlpha)
                {
                    _alpha = t.GetComponent<EnemyTypeRanger>();
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

    private void AddFollower(EnemyTypeRanger ranger)
    {
        if (!_isAlpha) return;
        _children.Add(ranger);
    }

    private void makeFollowersCircleTarget()
    {
        Debug.Log("Circle Target!");
        for (int i = 0; i < _children.Count; i++)
        {
            _children[i].MoveTo(new Vector3(
                _target.transform.position.x + _radiusSurroundTarget * Mathf.Cos(2 * Mathf.PI * i / _children.Count),
                _target.transform.position.y,
                _target.transform.position.z + _radiusSurroundTarget * Mathf.Sin(2 * Mathf.PI * i / _children.Count)));
        }
    }

    protected void UpdateClipLengths()
    {
        AnimationClip[] clips = _myAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "RangerAttackAnim":
                    attackLength = clip.length;
                    break;
                case "RangerDamagedAnim":
                    damageLength = clip.length;
                    break;
                case "RangerDiesAnim":
                    deathLength = clip.length;
                    break;
                case "RangerIdleAnim":
                    idleLength = clip.length;
                    break;
            }
        }
    }

}