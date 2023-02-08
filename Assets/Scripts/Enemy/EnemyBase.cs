using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _logState;
    [SerializeField, ReadOnly] public bool _active;
    [SerializeField] protected Targetable _target;
    [SerializeField] protected float _radiusSurroundTarget = 6f;
    
    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Health _health;
    [SerializeField] private EnemyData _data;
    [SerializeField] protected int _numberGloopsEaten = 0;
    
    /*
    [Header("My Animation")]
    [SerializeField] private Animator myAnimator = null;

    [Header("My Feedback")]
    [SerializeField] private ParticleSystem _spawnParticles;
    [SerializeField] private ParticleSystem _impactParticles;
    [SerializeField] private ParticleSystem _attackParticles;
    [SerializeField] private AudioSource _myAudioSource;
    [SerializeField] private AudioClip _moveSound;
    [SerializeField] private AudioClip _idleSound;
    [SerializeField] private AudioClip _spawnSound;
    [SerializeField] private AudioClip _impactSound;
    [SerializeField] private AudioClip _attackSound;
    */

    protected int distanceToRun = 40;
    private bool _hasTarget;

    public virtual EnemyData Data => _data;

    #region Unity Functions

    private void Awake()
    {
        //AIManager.Instance.Units.Add(this);
        _health.SetHealth(_data.MaxHealth);
    }

    private void OnEnable()
    {
        GameManager.OnPause += OnPause;
    }

    private void OnDisable()
    {
        GameManager.OnPause -= OnPause;
    }

    private void OnValidate()
    {
        if (!_agent) _agent = GetComponent<NavMeshAgent>();
        if (!_rb) _rb = GetComponent<Rigidbody>();
        if (!_health) _health = GetComponent<Health>();
    }

    private void Start()
    {
        //_playerTarget = PlayerManager.Instance.Player; //later make RANGED a possible target
        OnPause(false);
        _agent.speed = _data.MoveSpeed;
        OnStart();
    }

    private void Update()
    {
        if (!_active) return;
        if (_hasTarget && _target == null)
        {
            _hasTarget = false;
            OnLoseTarget();
        }
        if (!_hasTarget && _target != null)
        {
            _hasTarget = true;
        } 

        if (_health.health <= 0)
        {
            Log("Died");

            for (int i = 0; i < _data.Loot.Capacity; i++)
            {
                var obj = Instantiate(_data.Loot[i], gameObject.transform.position, Quaternion.identity);
                obj.name += " - " + name + " " + i;
            }
            Destroy(gameObject);
        }
        OnUpdate();
    }
    
    #endregion

    protected abstract void OnLoseTarget();
    protected abstract void OnStart();
    protected abstract void OnUpdate();

    private void OnPause(bool paused)
    {
        _active = !paused;
        _agent.enabled = !paused;
        if (_rb)
        {
            if (paused)
                _rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            else if (!paused)
                _rb.constraints = RigidbodyConstraints.None;
        }
        // BUG: Sometimes this makes the enemy fall through the floor?
    }

    protected void FaceTarget()
    {
        if (!_target) return;
        FacePosition(_target.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Data) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Data.RangeOfVision);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Data.AttackRange);
    }

    public void MoveTo(Vector3 newTarget)
    {
        NavMesh.SamplePosition(newTarget,out var hit, 10, NavMesh.AllAreas);
        _agent.SetDestination(hit.position);
        //Debug.Log("Targeting: " + _newTarget);
    }
    
    protected bool CheckTarget()
    {
        var targets = Targetable.GetAllWithinRange(transform.position, Data.RangeOfVision);
        var target = GetPotentialTarget(targets);
        _target = target;
        _hasTarget = _target;
        return _hasTarget;
    }

    protected abstract Targetable GetPotentialTarget(IEnumerable<Targetable> potentialTargets);
    
    protected bool CheckAttackTarget()
    {
        return (Vector3.Distance(transform.position, _target.transform.position) < Data.AttackRange);
    }

    protected void Log(string message)
    {
        if (_logState) Debug.Log("[" + name + "] " + message, gameObject);
    }

    protected void FacePosition(Vector3 pos)
    {
        Vector3 direction = (pos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}