using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibrarianBehavior : EnemyBase
{
    [SerializeField] Transform _teleport;
    [SerializeField, ReadOnly] private LibrarianState _librarianState;
    [SerializeField] float cooldown = 10f;

    float cooldownSubtract;


    protected override void OnStart()
    {
        cooldownSubtract = cooldown;
        _myAnimator = _AlphaFace.GetComponent<Animator>();
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
        //_teleport = DataManager.currentRoom.
        if (CheckTarget())
        {
            if (_target.Type == TargetableType.Player)
            {
                var playerMovement = _target.GetComponent<PlayerMovementScript>();
                playerMovement._moveSpeed = playerMovement._slowedSpeed;
                if (DataManager.bIsHostile)
                    TrySetState(LibrarianState.Chasing);
            }
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


    }

    private void OnChasingState()
    {
        if (!_target)
        {   
            TrySetState(LibrarianState.Standing);
            return;
        }

        if (_target.Type == TargetableType.Player)
        {
            //_musicPlayer.clip = _myTheme;
            MoveTo(_target.transform.position + new Vector3(5 / 2, 0, 5 / 2));
        }

        if ((Time.time - cooldown) > Data.MemoryTimeout)
        {
            
                TrySetState(LibrarianState.Standing);
                OnLoseTarget();
                return;
        }
    }

    private void Teleport()
    {
        var room = DataManager.currentRoom;
        if ((room.HalfRoomSize.x * 2)> 15 && (room.HalfRoomSize.y * 2) > 15)
        {
            float x = (room.HalfRoomSize.x - 4) * (Random.value > 0.5f ? 1 : -1);
            float z = (room.HalfRoomSize.y - 4) * (Random.value > 0.5f ? 1 : -1);
            transform.position = room.transform.position + new Vector3(x, 0, z);
            Debug.Log(transform.position);
        }
    
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

        Debug.Log("Librarian lost target");
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
}

public enum LibrarianState
{
    Standing,
    Chasing
}
