using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
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
    private Player _player; //do I need this if player is Singleton?
    Transform _target;
    NavMeshAgent _agent;



    // Start is called before the first frame update
    void Start()
    {
        _target = PlayerManager.instance.player.transform;
        Debug.Log(_target);
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = _moveSpeed;
        
    }

    // Update is called once per frame
    void Update()
    {
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
