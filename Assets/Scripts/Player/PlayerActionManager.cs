using Cinemachine;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    // The hub that translates Player Input to all other scripts in the game
    
    [SerializeField] private bool _logInput;
    [SerializeField] private bool _logState;
    [SerializeField, ReadOnly] private PlayerState _playerState;
    [SerializeField] private PlayerMovementScript _movement;
    [SerializeField] private MouseRotation _lookRotation;
    [SerializeField] private PlayerPerspectiveChange _perspective;
    [SerializeField] private CinemachineInputProvider _cameraInput;
    
    public PlayerState State => _playerState;
    private bool InGame => State == PlayerState.InGame;
    
    private void Start()
    {
        CheckState();
    }

    public void Move(Vector2 moveDir)
    {
        _movement.SetMoveDir(moveDir);
    }

    public void Jump()
    {
        LogInput("Jump");
        _movement.SetToJump();
    }

    public void Sprint(bool sprinting)
    {
        LogInput($"Sprinting: {sprinting}");
        _movement.SetSprinting(sprinting);
    }

    public void Crouch(bool crouching)
    {
        LogInput($"Crouching: {crouching}");
        _movement.SetCrouching(crouching);
    }

    public void Look(Vector2 look)
    {
        _lookRotation.SetLookDir(look);
    }

    public void TogglePerspective()
    {
        _perspective.TogglePerspective();
    }

    public void Attack()
    {
        LogInput("Attack");
    }

    public void Interact()
    {
        LogInput("Interact");
    }

    public void OpenPauseMenu()
    {
        LogInput("Open Pause Menu");
        CanvasController.Singleton.OpenPauseMenu();
    }

    public void OpenInventory()
    {
        LogInput("Open Inventory");
        CanvasController.Singleton.OpenInventory();
    }

    public void CloseAnyMenu()
    {
        LogInput("Close Any Menu");
        CanvasController.Singleton.CloseMenu();
    }

    public void NextItem()
    {
        LogInput("Switch to Next Item");
        CanvasController.ToolbarManager.SelectNextItem();
    }

    public void PreviousItem()
    {
        LogInput("Switch to Previous Item");
        CanvasController.ToolbarManager.SelectPreviousItem();
    }

    public void SelectItem(int item)
    {
        LogInput($"Select Item Slot {item}");
        CanvasController.ToolbarManager.SelectItem(item);
    }

    public void CheckState()
    {
        if (CanvasController.Singleton.PauseMenuOpen)
        {
            TrySetState(PlayerState.InPauseMenu);
        }
        else if (CanvasController.Singleton.InventoryOpen)
        {
            TrySetState(PlayerState.InInventory);
        }
        else
        {
            TrySetState(PlayerState.InGame);
        }
        Cursor.lockState = InGame ? CursorLockMode.Locked : CursorLockMode.None;
        _cameraInput.enabled = InGame;
        if (!InGame)
        {
            _movement.SetMoveDir(Vector2.zero);
        }
        GameManager.Instance.SetPaused(!InGame);
    }
    
    private bool TrySetState(PlayerState newState)
    {
        if (newState == State) return false;
        if (_logState) Debug.Log($"Player State switched to {newState}", gameObject);
        _playerState = newState;
        return true;
    }

    private void LogInput(string message)
    {
        if (_logInput) Debug.Log(message, gameObject);
    }
}