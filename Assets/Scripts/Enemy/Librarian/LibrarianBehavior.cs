using System.Collections;
using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

public class LibrarianBehavior : EnemyBase
{
    [SerializeField] Transform _teleport;
    [SerializeField, ReadOnly] private LibrarianState _librarianState;
    [SerializeField] float cooldown = 20f;
    [SerializeField] GameObject librarianChaser;
    [SerializeField] private Room _currentRoom;
    AudioSource[] myAudioSources;

    float cooldownSubtract;
    bool _hasDisappeared = false;

    protected override void OnAwake()
    {
        _agent.speed = Data.MoveSpeed;
        Teleport();
        _myAnimator = _AlphaFace.GetComponent<Animator>();
        myAudioSources = GetComponents<AudioSource>();
    }

    protected override void OnStart()
    {
        cooldownSubtract = cooldown;
        //_spawnSound.Play();
        Teleport();
        CheckTarget();
        
        //_musicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();


    }

    protected override void OnUpdate()
    {

        switch (_librarianState)
        {
            default:
            case LibrarianState.Standing:
                OnStandingState();
                break;

            case LibrarianState.Chasing:
                OnChasingState();
                break;
        }
    }

    private void OnStandingState()
    {
        //_teleport = GameManager.Data.currentRoom.
        if (CheckTarget())
        {
            if (_target.Type == TargetableType.Player)
            {
                var playerMovement = _target.GetComponent<PlayerMovementScript>();
                playerMovement._moveSpeed = playerMovement._slowedSpeed;
                //if (GameManager.Data.bIsHostile)
                //    TrySetState(LibrarianState.Chasing);
            }
        }

        if (_currentRoom != GameManager.Data.CurrentRoom)
        {
            Teleport();
        }

        cooldownSubtract -= Time.deltaTime;
        if (cooldownSubtract <= 0f)
        {
            if (_target == null)
            {
                Teleport();
                cooldownSubtract = cooldown;
                return;
            }
        }

        if (GameManager.Data.BIsHostile && !_hasDisappeared)
        {
            Timer.DelayAction(this, ChasePlayer, cooldown);
            _hasDisappeared = true;
        }


    }

    private void OnChasingState()
    {
        
        if (!_target)
        {
            OnLoseTarget();
            return;
        }

        
        if (_target.Type == TargetableType.Player)
        {
            //_musicPlayer.clip = _myTheme;
            MoveTo(_target.transform.position + new Vector3(5 / 2, 0, 5 / 2));

            if (Vector3.Distance(transform.position, _target.transform.position) < 15f)
            {
                Appear();
                if (_currentRoom != GameManager.Data.CurrentRoom)
                {
                    _agent.Warp(_target.transform.position - new Vector3(5, 0, 5));
                    _currentRoom = GameManager.Data.CurrentRoom;
                }
            }

            if (Vector3.Distance(transform.position, _target.transform.position) < 3f)
            {
                PlayerManager.Instance.Survival.Decrease(SurvivalStatEnum.Health, Random.Range(Data.AttackDamage / 1.3f, Data.AttackDamage));
            }
            //if (librarianChaser) Instantiate(librarianChaser, this.transform, false);
            //Destroy(this.gameObject);
        }

        Timer.DelayAction(this, ResetLibrarian, Data.MemoryTimeout);
        //if ((Time.time - cooldown) > Data.MemoryTimeout)
        //{
        //    
        //        TrySetState(LibrarianState.Standing);
        //        _BetaFace.SetActive(false);
        //        _AlphaFace.SetActive(true);
        //        _agent.enabled = false;
        //        OnLoseTarget();
        //        return;
        //}
    }

    public void Teleport()
    {
        if (GameManager.Data.CurrentRoom.Hallway) return;
        _currentRoom = GameManager.Data.CurrentRoom;
        var room = GameManager.Data.CurrentRoom;
        float x = (room.HalfRoomSize.x - 4) * (Random.value > 0.5f ? 1 : -1);
        float z = (room.HalfRoomSize.y - 4) * (Random.value > 0.5f ? 1 : -1);
        transform.position = room.transform.position + new Vector3(x, 0, z);
        _agent.Warp(room.transform.position + new Vector3(x, 0, z));
        Debug.Log(transform.position);
    }

    public void Disappear()
    {
        _AlphaFace.SetActive(false);

        foreach(AudioSource _source in myAudioSources)
        {
            _source.enabled = false;
        }
    }

    public void Appear()
    {
        _AlphaFace.SetActive(true);
        foreach (AudioSource _source in myAudioSources)
        {
            _source.enabled = true;
        }
    }

    public void ResetLibrarian()
    {
        if (_librarianState != LibrarianState.Standing)
        {
            Debug.Log("Reset Librarian");
            _BetaFace.SetActive(false);
            _AlphaFace.SetActive(true);
            //_agent.enabled = false;
            _hasDisappeared = false;
            Teleport();
            //OnLoseTarget();
            TrySetState(LibrarianState.Standing);
            return;
        }
        
    }

    public void ChasePlayer()
    {
        StartCoroutine(WaitInShadows(20));
    }

    private bool TrySetState(LibrarianState newState, bool fromAlpha = false)
    {

        if (newState == _librarianState) return false;
        Log($"State switched to {newState}");
        _librarianState = newState;
  
        return true;//runInEditMode 
    }

    protected override void OnLoseTarget()
    {
        if (_target)
        {
            var playerMovement = _target.GetComponent<PlayerMovementScript>();
            playerMovement._moveSpeed = playerMovement._defaultMoveSpeed;
        }

        _target = null;

        //Teleport();
        Timer.DelayAction(this, ResetLibrarian, Data.MemoryTimeout);
        //Teleport();
    }


    protected override Targetable GetPotentialTarget(IEnumerable<Targetable> targets)
    {
        Targetable target = null;
        int targetPriority = -1;
        foreach (Targetable t in targets)
        {
            
            if (targetPriority < 5 && t.Type == TargetableType.Player)
            {
                target = t;
                targetPriority = 5;
            }
           
        }
        return target;
    }

    public IEnumerator WaitInShadows(float waitTime)
    {
        Disappear();
        yield return new WaitForSeconds(waitTime);
        Appear();

        if (_librarianState != LibrarianState.Chasing)
        {
            _BetaFace.SetActive(true);
            _AlphaFace.SetActive(false);
           // _agent.enabled = true;
            cooldown = Time.deltaTime;
            Timer.DelayAction(this, ResetLibrarian, 100f);
            TrySetState(LibrarianState.Chasing);
        }

        yield return new WaitForSeconds(0.1f);

    }
}

public enum LibrarianState
{
    Standing,
    Chasing
}
