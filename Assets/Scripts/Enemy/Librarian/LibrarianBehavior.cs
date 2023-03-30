using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibrarianBehavior : EnemyBase
{
    [SerializeField] Transform _teleport;
    [SerializeField, ReadOnly] private LibrarianState _librarianState;


    protected override void OnStart()
    {

        _myAnimator = _AlphaFace.GetComponent<Animator>();
        _musicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();
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

        if (_target.Type == TargetableType.Player)
        {
            var playerMovement = _target.GetComponent<PlayerMovementScript>();
            playerMovement._moveSpeed = playerMovement._slowedSpeed;
            if (DataManager.bIsHostile)
                TrySetState(LibrarianState.Chasing);
        }
    }

    private void OnChasingState()
    {
        _musicPlayer.clip = _myTheme;
        MoveTo(_target.transform.position + new Vector3(5 / 2, 0, 5 / 2));
    }


    private bool TrySetState(LibrarianState newState, bool fromAlpha = false)
    {

        if (newState == _librarianState) return false;
        Log($"State switched to {newState}");
        _librarianState = newState;
  
        return true;
    }

    protected override void OnLoseTarget()
    {
        _target = null;
    }


    protected override Targetable GetPotentialTarget(IEnumerable<Targetable> potentialTargets)
    {
        throw new System.NotImplementedException();
    }
}

public enum LibrarianState
{
    Standing,
    Chasing
}
