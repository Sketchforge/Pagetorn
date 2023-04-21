using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool _logState;
    [SerializeField, ReadOnly] public bool _active;
    [SerializeField] protected Targetable _target;
    public bool onlyRotateY = true;
    public bool useStaticBillboard = false;
    
    [Header("References")]
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Health _health;
    [SerializeField] private EnemyData _data;
    [SerializeField] protected int _numberGloopsEaten = 0;
    [SerializeField] protected GameObject _BetaFace;
    [SerializeField] protected GameObject _AlphaFace;
    [SerializeField] private Camera theCam;

    [SerializeField] LayerMask hitMask;

    /*
    [Header("My Animation")]
    [SerializeField] private Animator myAnimator = null;
    */

    [Header("My Feedback")]
    [SerializeField] protected ParticleSystem _spawnParticles;
    [SerializeField] protected ParticleSystem _impactParticles;
    [SerializeField] protected ParticleSystem _attackParticles;
    [SerializeField] protected SfxReference _moveSound;
    [SerializeField] protected SfxReference _idleSound;
    [SerializeField] protected SfxReference _spawnSound;
    [SerializeField] protected SfxReference _impactSound;
    [SerializeField] protected SfxReference _attackSound;

    [SerializeField] protected SoundEvent _chaseMusic;

    // [SerializeField] public AudioSource _musicPlayer;
    // [SerializeField] public AudioClip _myTheme;


    //ANIMATION
    protected Animator _myAnimator;
    protected float attackLength;
    protected float damageLength;
    protected float deathLength;
    protected float idleLength;


    protected int distanceToRun = 40;
    private bool _hasTarget;

    public virtual EnemyData Data => _data;

    #region Unity Functions

    private void Awake()
    {
        //AIManager.Instance.Units.Add(this);
        _health.SetHealth(_data.MaxHealth);
        theCam = Camera.main;
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
        if (_target)
        {
            if (_health.health <= 0)
            {
                Log("Died");
                if (_target.Type == TargetableType.Player)
                {
                    DataManager.NumberMonstersKilled++;
                    DataManager.NumberMonstersKilledLastHour++;
                }

                for (int i = 0; i < _data.Loot.Capacity; i++)
                {
                    var obj = Instantiate(_data.Loot[i], gameObject.transform.position, Quaternion.identity);
                    obj.name += " - " + name + " " + i;
                }
                Die();
            }
        }
        
        OnUpdate();
    }

    private void LateUpdate()
    {
        BillboardFace(_AlphaFace.transform);
        BillboardFace(_BetaFace.transform);
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

    protected virtual void Die()
    {
        //this.gameObject.SetActive(false);
        Destroy(gameObject);
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

    protected void BillboardFace(Transform myFace)
    {
        if (!theCam) return;
        if (!useStaticBillboard)
        {
            transform.LookAt(theCam.transform);
	        //Debug.Log("isRotatingFace");
        }
        else
        {
            transform.rotation = theCam.transform.rotation;
        }

        if (onlyRotateY)
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.layer == hitMask)
    //    {
    //        Debug.Log("Enemy Hit by " + collision.gameObject);
    //        GetComponent<Health>().Damage(collision.gameObject.GetComponent<WeaponStats>().damage);
    //    }
    //}

    public void KnockBack(float amount, Vector3 direction)
	{
        _agent.updatePosition = false;
        _rb.AddForce(direction * amount, ForceMode.Impulse);
        _agent.updatePosition = true;
    }
    
}