using System;
using Cinemachine;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    // The hub that translates Player Input to all other scripts in the game
    
    [SerializeField] private bool _logInput;
    [SerializeField] private bool _logMovement;
    [SerializeField] private bool _logState;
    [SerializeField, ReadOnly] private PlayerState _playerState;
    [SerializeField] private PlayerMovementScript _movement;
    [SerializeField] private MouseRotation _lookRotation;
    [SerializeField] private MouseRotation _lookBodyRotation;
    
    public PlayerState State => _playerState;
    private bool InGame => State == PlayerState.InGame;
    
    private void Start()
    {
        CheckState();
    }

    public void Move(Vector2 moveDir)
    {
        if (_logMovement) Debug.Log("Move: " + moveDir, gameObject);
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
        if (_logMovement) Debug.Log("Look: " + look, gameObject);
        _lookBodyRotation.SetLookDir(look);
        _lookRotation.SetLookDir(look);
    }

    public void TogglePerspective()
    {
        LogInput("Toggle Perspective");
    }

    public void Attack()
    {
        LogInput("Attack");
        var itemInHand = CanvasController.ToolbarManager.SelectedItem;
        switch (itemInHand.Type)
        {
            case ItemType.Blade:
            case ItemType.Hammer:
            case ItemType.Tool:
                // Try to attack with weapon
                break;
            default:
                // Basic / weak attack with item in hand?
                break;
        }
    }

    public void Interact()
    {
        LogInput("Interact");
        var itemInHand = CanvasController.ToolbarManager.SelectedItem;
        switch (itemInHand.Type)
        {
            case ItemType.Tool:
                // Try to use tool
                break;
            case ItemType.Consumables:
                // Consume
                break;
            default:
                // Check for interaction with world
                break;
        }
        // Maybe some items can be placed as well?
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