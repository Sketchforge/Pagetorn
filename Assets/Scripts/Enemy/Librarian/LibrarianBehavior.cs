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
            if (!CheckTarget())
            {
                Teleport();
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
            _musicPlayer.clip = _myTheme;
            MoveTo(_target.transform.position + new Vector3(5 / 2, 0, 5 / 2));
        }

        if ((Time.time - cooldown) > Data.MemoryTimeout)
        {
            
                TrySetState(LibrarianState.Standing);
                return;
        }
    }

    private void Teleport()
    {
        int randomCorner = Mathf.CeilToInt(Random.Range(0, 2));
        if (randomCorner == 1)
        {
            transform.position = DataManager.currentRoom.transform.position + new Vector3(DataManager.currentRoom.HalfRoomSize.x - 3f, 0, DataManager.currentRoom.HalfRoomSize.y - 3f);
        }
        else
        {
            transform.position = DataManager.currentRoom.transform.position - new Vector3(DataManager.currentRoom.HalfRoomSize.x + 3f, 0, DataManager.currentRoom.HalfRoomSize.y + 3f);
        }
       
        cooldownSubtract = cooldown;
        Debug.Log(transform.position);
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
        _target = null;
        var playerMovement = _target.GetComponent<PlayerMovementScript>();
        playerMovement._moveSpeed = playerMovement._defaultMoveSpeed;
        Debug.Log("Librarian lost target");
        Teleport();
    }


    protected override Targetable GetPotentialTarget(IEnumerable<Targetable> targets)
    {
        Targetable target = null;
        int targetPriority = -1;
        foreach (Targetable t in targets)
        {
            //if (targetPriority < 10 && t.Type == TargetableType.Witch)
            //{
            //    target = t; //target to run FROM librarian
            //    targetPriority = 10;
            //}
            if (targetPriority < 5 && t.Type == TargetableType.Player)
            {
                target = t;
                targetPriority = 5;
            }
            //if (t.Type == TargetableType.AlphaCrawler && t.transform != transform)
            //{
            //    if (!_isAlpha)
            //    {
            //        _alpha = t.GetComponent<EnemyTypeCrawler>();
            //        if (HasAlpha) _alpha.AddFollower(this);
            //    }
            //    if (targetPriority < 1)
            //    {
            //        target = t;
            //        targetPriority = 1;
            //    }
            //}
            //if (targetPriority < 0 && t.Type == TargetableType.Gloop)
            //{
            //    target = t;
            //    targetPriority = 0;
            //}
        }
        return target;
    }
}

public enum LibrarianState
{
    Standing,
    Chasing
}
