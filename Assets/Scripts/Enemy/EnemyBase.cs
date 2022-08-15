using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyBase : MonoBehaviour
{
    [SerializeField, ReadOnly] public bool _active;

    [SerializeField] protected GameObject nextStage;

    [Header("My Stats")]
    [SerializeField] float _maxHealth = 100f;
    [SerializeField] protected float _attackDamage = 10f;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] protected float _rateOfAttack = 2f;
    [SerializeField] protected int _numberGloopsEaten = 0;

    [Header("My Vision")]
    [SerializeField] protected float _rangeOfVision = 50;
    [SerializeField] protected float _attackRange = 30;
    [SerializeField] protected float _memoryTimeout = 5;
    //TODO: Add tool to make vision cones, cubes, or spheres, rather than just having a sphere.

    [Header("My Loot")]
    [SerializeField] protected List<GameObject> Loot;


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
    [SerializeField] protected Transform _target;
    public NavMeshAgent _agent;
    private Rigidbody _myRb;
    public bool seesTarget = false;
    protected bool atTarget = false;
    public Health _myHealth;

    protected int distanceToRun = 40;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _myRb = GetComponent<Rigidbody>();
        _myHealth = GetComponent<Health>();
        _myHealth.SetHealth(_maxHealth);
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
        //_playerTarget = PlayerManager.Instance.Player; //later make RANGED a possible target
        OnPause(false);
        _agent.speed = _moveSpeed;
    }

    protected virtual void Update()
    {
        if (!_active) return; 
        
        if (_myHealth.health <= 0)
        {
            Debug.Log("I am an enemy. I am dead. Damn");

            for (int i = 0; i < Loot.Capacity; i++)
            {
                Instantiate(Loot[i], gameObject.transform.position, Quaternion.identity);
            }

            gameObject.SetActive(false);
            _active = false;
            //Destroy(this.gameObject);
        }
        
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
        if (_target)
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _rangeOfVision);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }


    protected void MoveTo(Vector3 _newTarget)
    {
        NavMesh.SamplePosition(_newTarget,out var hit, 10, NavMesh.AllAreas);
        _agent.SetDestination(hit.position);
        //Debug.Log("Targeting: " + _newTarget);

    }

    protected void FindTarget()
    {
        Collider[] checkTargets = Physics.OverlapSphere(gameObject.transform.position, _rangeOfVision);
        foreach (Collider collider in checkTargets)
        {
            if (collider.tag == "Witch")
            {
                _target = collider.transform; //target to run FROM librarian
                break;
            }
            if (collider.tag == "Player")
            {
                _target = PlayerManager.Instance.Player;
                break;
            }
            if (collider.tag == "AlphaCrawler" && collider.transform != this.transform)
            {
                _target = collider.transform;
                break;
            }
            if (collider.tag == "Gloop")
            {
                _target = collider.transform;
                break;
            }
        }
    }

    protected bool CheckTarget()
    {
        FindTarget();
        return (Vector3.Distance(transform.position, _target.position) <= _rangeOfVision);
    }

    protected bool CheckAttackTarget()
    {
        return (Vector3.Distance(transform.position, _target.position) < _attackRange);
    }

}