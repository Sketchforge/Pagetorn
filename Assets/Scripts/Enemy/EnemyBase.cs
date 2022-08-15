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
    [SerializeField] float _rangeOfVision;
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
    protected Transform _target;
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

    void Start()
    {
        _target = PlayerManager.Instance.Player; //later make RANGED a possible target
        Debug.Log(_target);

        _agent.speed = _moveSpeed;
    }

    void Update()
    {
        if (!_active) return;

        float distance = Vector3.Distance(_target.position, transform.position);

        if (distance <= _rangeOfVision)
        {
            seesTarget = true;
            MoveTo(_target.position);

            if (distance <= _agent.stoppingDistance)
            {
                atTarget = true;
                //Attack the target if Player or Natural Enemy
                //Face the target if Player, Natural Enemy, or Glob
                //Eat target if Glob.
                FaceTarget();
            }
            else if (distance < _agent.stoppingDistance)
            {
                atTarget = false;
            }
        }
        else if (distance > _rangeOfVision)
            seesTarget = false;
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

    void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _rangeOfVision);
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

    protected void MoveTo(Vector3 _target)
    {
        _agent.SetDestination(_target);
       
    }

}