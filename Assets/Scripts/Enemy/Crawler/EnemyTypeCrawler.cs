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
    [SerializeField] protected float _radiusSurroundTarget = 6f;

    [Header("Debug")]
    [SerializeField, ReadOnly] private Vector3 _startingPosition;
    [SerializeField, ReadOnly] private Vector3 _roamPosition;
    [SerializeField, ReadOnly] private float _attackTime;
    [SerializeField, ReadOnly] private float _memoryTime;
    [SerializeField, ReadOnly] private float _roamTime;
    [SerializeField] private bool hasRoamPos = true;
    private bool _themePlaying = false;

   

    public override EnemyData Data => _isAlpha ? _alphaData : base.Data;

    public bool HasAlpha => _alpha != null;

    protected override void OnStart()
    {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        UpdateAlphaStatus();

        _myAnimator = _BetaFace.GetComponent<Animator>();
        UpdateClipLengths();

       // _musicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();
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
        //Debug.Log("RoamState");
        _memoryTime = Time.time;
        if (Vector3.Distance(transform.position, _roamPosition) < 20f)
        {
            _roamPosition = GetRoamingPosition();
        }
        FacePosition(_roamPosition);
        MoveTo(_roamPosition);
        //if (_moveSound.Clip.length - Time.deltaTime > 0)

        if (_moveSound.Clip.length < Time.deltaTime)
        {

        }
        _moveSound.PlayAtParentAndFollow(this.gameObject.transform);
        if (Vector3.Distance(transform.position, _roamPosition) < 0.5f || (HasAlpha && Vector3.Distance(transform.position, _alpha.transform.position) > 10f))
        {
            //Debug.Log("RoamStateIsBeingGlitchy");
            _roamPosition = GetRoamingPosition();
        }

        if (CheckTarget()) TrySetState(CrawlerState.Chasing);
    }
    
    private void OnChasingState()
    {
        //Debug.Log("ChaseState");
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

        if (_target.Type == TargetableType.Gloop && !HasAlpha)
        {
            _randomFollowRange = new Vector2(0.1f, 1f);
        }

        MoveTo(_target.transform.position + new Vector3(_randomFollowRange.x/2,0, _randomFollowRange.y/2));
        _moveSound.PlayAtParentAndFollow(this.transform);
        if (_isAlpha)
            makeFollowersCircleTarget();

        if ((Time.time - _memoryTime) > Data.MemoryTimeout)
        {
            if (!CheckTarget())
            {
                TrySetState(CrawlerState.Roaming);
                return;
            }
        }


        if (_target.Type == TargetableType.Player)
        {
            //if (_musicPlayer.clip != _myTheme)
            //{
            //    _musicPlayer.clip = _myTheme;
            //    _musicPlayer.Play();
            //}
            if (!_themePlaying)
            {
                _chaseMusic.ActivateEvent();
                _themePlaying = true;
            }
            

        }

        _attackTime = Time.time;

        if (CheckAttackTarget())
        {
            TrySetState(CrawlerState.Attacking);
        }
    }
    
    private void OnAttackingState()
    {
        //Debug.Log("AttackState");
        if (!_target)
        {
            TrySetState(CrawlerState.Roaming);
            return;
        }
        FaceTarget();
        if (_target.Type == TargetableType.Player)
        {
            if (!_isAlpha)
            {
                if ((Time.time - _attackTime) > Random.Range(Data.RateOfAttack - 1f, Data.RateOfAttack)) //if not an alpha, simply deal damage
                {
                    float timeElapsed = 0f;
                    timeElapsed += Time.time;
                    _myAnimator.SetTrigger("Attack");
                    if (timeElapsed >= attackLength/2)
                    {
                        Collider[] hitInfo = Physics.OverlapSphere(_BetaFace.transform.position, 10);
                        foreach(Collider _collider in hitInfo)
                        {
                            if (_collider.GetComponent<PlayerMovementScript>() != null)
                            {
                                _attackSound.PlayAtPosition(transform.position);
                                PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, Random.Range(Data.AttackDamage / 2, Data.AttackDamage)); //switch w/ hitbox and animation later 
                                Debug.Log("Hit Player");
                                _attackTime = Time.time;
                                TrySetState(CrawlerState.Roaming);
                            }
                        }
                        
                    }
                    
                }
            }
        }
        if (_target.Type == TargetableType.AlphaCrawler)
        {
            if ((Time.time - _attackTime) > Random.Range(Data.RateOfAttack - 1f, Data.RateOfAttack))
            {
                _target.GetComponent<Health>().Damage(Mathf.CeilToInt(Random.Range(Data.AttackDamage / 2, Data.AttackDamage)));
                _attackTime = Time.time;
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
        float distance = 1000;
        if (_target)
        {
            distance = Vector3.Distance(transform.position, _target.transform.position);
        }
         

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
        
        var rand = Random.insideUnitSphere * GetRandom(HasAlpha ? _randomFollowRange : _randomRoamRange);
        rand.y = 0;
        _roamTime = Time.time;
        if (HasAlpha)
        {
            hasRoamPos = false;
            return rand + _alpha.transform.position + -_alpha.transform.forward * Random.Range(0.5f, 2.5f);
        }
        else
        {
            hasRoamPos = true;
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

    private void makeFollowersCircleTarget()
    {
	    //Debug.Log("Circle Target!");
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
                case "CrawlerAttackAnim":
                    attackLength = clip.length;
                    break;
                case "CrawlerDamagedAnim":
                    damageLength = clip.length;
                    break;
                case "CrawlerDieAnim":
                    deathLength = clip.length;
                    break;
                case "CrawlerIdleAnim":
                    idleLength = clip.length;
                    break;
            }
        }
    }

  //  protected override void Die()
  //  {
  //      if (_isAlpha)
  //      {
  //          for (int i = 0; i < _children.Count; i++)
  //          {
  //              Destroy(_children[i].gameObject);
  //          }
  //          Destroy(this.gameObject);
  //      }
  //  }

}