using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool _active;

    [Header("My Stats")]
    [SerializeField] float _health = 100f; //to be replaced with reference to general Health/Damageable script.
    [SerializeField] float _attackDamage = 1f;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _rateOfAttack = 1f;

    [Header("My Vision")]
    [SerializeField] float _rangeOfVision = 30f;
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
    Transform _target;
    NavMeshAgent _agent;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        GameManger.OnPause += OnPause;
    }

    private void OnDisable()
    {
        GameManger.OnPause -= OnPause;
    }

    void Start()
    {
        _target = PlayerManager.Instance.Player;
        Debug.Log(_target);

        _agent.speed = _moveSpeed;
    }

    void Update()
    {
        if (!_active) return;

        float distance = Vector3.Distance(_target.position, transform.position);

        if (distance <= _rangeOfVision)
        {
            _agent.SetDestination(_target.position);

            if (distance <= _agent.stoppingDistance)
            {
                //Attack the target
                //Face the target
                FaceTarget();
            }
        }
    }

    private void OnPause(bool paused)
    {
        _active = !paused;
        _agent.enabled = !paused;

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
}