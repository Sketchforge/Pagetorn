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
    [SerializeField] private Transform _lookDir;
    [SerializeField] private float _interactDistance = 4;
    
    [Header("Feedback")]
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private GameObject _heldItemSocket;
    [SerializeField] private SfxReference _hammerSwingSfx;

    [Header("Headbob and FootSteps")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.5f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.2f;
    private float defaultYpos = 0;
    private float headbobTimer;
    private bool canAttack = false;


    public PlayerState State => _playerState;
    private bool InGame => State == PlayerState.InGame;
    public Transform LookDirection => _lookDir;
    
    private void Start()
    {
        CheckState();
    }

    private void Update()
    {
        DataManager.totalTime += Time.deltaTime;
        DataManager.AmountTimeStoodStill += Time.deltaTime;
    }

    public void Move(Vector2 moveDir)
    {
        if (_logMovement) Debug.Log("Move: " + moveDir, gameObject);
        DataManager.AmountTimeStoodStill = 0;
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

    public void SetAttack(bool value)
	{
        canAttack = value;
	}

    public void Attack()
    {
        switch (CanvasController.ToolbarManager.SelectedItemType)
        {
            case ItemType.Blade:
            case ItemType.Hammer:
                // Try to attack with weapon
                LogInput("Attack (Weapon)");
                //if (_myAnimator)
                    
                var currentWeapon = CanvasController.ToolbarManager.SelectedItem;
                if (currentWeapon.IsToolOrWeapon)
                {
                    var weaponAnim = _heldItemSocket.GetComponentInChildren<Animator>(); //temporary? might not be great to use animators on all prefabs
                    if (weaponAnim)
                    {
                        if (canAttack)
                        {
                            _hammerSwingSfx.Play();
                            weaponAnim.SetTrigger("Swing");
                            DataManager.NumberMeleeAttacksDone++;
                        }
                    }
                }
                //if (currentWeapon != null) currentWeapon.UseWeaponTrigger("Swing 1");



                break;
            case ItemType.Tool:
                // Try to attack with weapon
                LogInput("Attack (Tool)");
                //var currentTool = CanvasController.ToolbarManager.SelectedItem;
                
                break;
            case ItemType.Magic:
                // Try to use spell
                LogInput("Attack (Spell)");
                DataManager.NumberSpellsDone++;
                DataManager.NumberSpellsDoneLastMinute++;
                var magic = (Magic)CanvasController.ToolbarManager.SelectedItem;
                if (magic != null) magic.CastSpell(CanvasController.ToolbarManager.SelectedItemSlot);
                break;
            default:
                // Basic / weak attack with item in hand?
                LogInput("Attack (Hand)");
                break;
        }
    }

    public void Interact()
    {
        switch (CanvasController.ToolbarManager.SelectedItemType)
        {
            case ItemType.Tool:
                // Try to use tool
                LogInput("Interact (Tool)");
                break;
            case ItemType.Consumables:
                // Consume
                LogInput("Interact (Consumable)");
                break;
            default:
                // Check for interaction with world
                Physics.Raycast(_lookDir.position, _lookDir.forward, out var hit, _interactDistance);
                if (hit.transform != null)
                {
                    // Hit Something
                    LogInput($"Attempting to Interact with {hit.transform.gameObject}");
                    var interactable = hit.transform.GetComponent<PlayerInteractable>();
                    if (interactable != null)
                    {
                        interactable.Interact();
                    }
                }
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