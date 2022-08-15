using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField, ReadOnly] protected bool _active;

    [Header("My Stats")]
    [SerializeField] private EnemyData data;
    [SerializeField] float _health;
    [SerializeField] float _attackDamage;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _rateOfAttack;

    [Header("My Vision")]
    [SerializeField] protected float _rangeOfVision;
    [SerializeField] protected float _attackRange;
    //TODO: Add tool to make vision cones, cubes, or spheres, rather than just having a sphere.

    [Header("My Animation")]
    [SerializeField] Animator myAnimator = null;

    [Header("My Feedback")]
    [SerializeField] ParticleSystem _spawnParticles;
    [SerializeField] ParticleSystem _impactParticles;
    [SerializeField] ParticleSystem _attackParticles;
    [SerializeField] AudioSource _myAudioSource;
    [SerializeField] AudioClip _moveSound;
    [SerializeField] AudioClip _idleSound;
    [SerializeField] AudioClip _spawnSound;
    [SerializeField] AudioClip _impactSound;
    [SerializeField] AudioClip _attackSound;

    [Header("Necessary Data")]
    private PlayerManager _playerManager; //do I need this if player is Singleton?
    protected Transform _playerTarget;
    protected NavMeshAgent _agent;
    private Rigidbody _myRb;
    public bool seesTarget = false;
    protected bool atTarget = false;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _myRb = GetComponent<Rigidbody>();     
    }

    private void OnEnable()
    {
        GameManager.OnPause += OnPause;
    }

    private void OnDisable()
    {
        GameManager.OnPause -= OnPause;
    }

    protected virtual void Start()
    {
        _playerTarget = PlayerManager.Instance.Player; //later make RANGED a possible target
        
        _agent.speed = _moveSpeed;
    }

    protected virtual void Update()
    {
        if (!_active) return; 
    }

    private void OnPause(bool paused)
    {
        _active = !paused;
        _agent.enabled = !paused;
        if (_myRb)
        {
            if (paused)
                _myRb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            else if (!paused)
                _myRb.constraints = RigidbodyConstraints.None;
        }

        // BUG: Sometimes this makes the enemy fall through the floor?
    }

    protected void FaceTarget()
    {
        Vector3 direction = (_playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _rangeOfVision);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    void SetEnemyValues()
    {
        
        GetComponent<Health>().SetHealth(data.HP);
        _attackDamage = data.damage;    
        _moveSpeed = data.moveSpeed;
        _rateOfAttack = data.attackRate;
        _rangeOfVision = data.rangeOfVision;
        Debug.Log("Set Enemy Values!");
    }

    protected void MoveTo(Vector3 _newTarget)
    {
        NavMesh.SamplePosition(_newTarget,out var hit, 10, NavMesh.AllAreas);
        _agent.SetDestination(hit.position);
        Debug.Log("Targeting: " + _newTarget);

    }

    protected bool FindPlayer()
    {
        return (Vector3.Distance(transform.position, _playerTarget.position) <= _rangeOfVision);
    }

    protected bool CheckAttackTarget()
    {
        return (Vector3.Distance(transform.position, _playerTarget.position) < _attackRange);
    }

}