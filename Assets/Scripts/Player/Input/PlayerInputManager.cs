using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    // Handles ALL Input with the Unity Input System.
    // Basically, any input set up on PlayerInput refers to a function below with On appended to the start
    // Uses the PlayerActionManager to control the logic of whether actions should be passed through or not
    // See PlayerActionManager for all implementations
    
    [SerializeField] private bool _reverseScrollDirection;
    [SerializeField] private float _mouseSensitivity = 1;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private PlayerActionManager _actions;
    
    public static Vector3 MousePos => Mouse.current.position.ReadValue();
    private bool InGame => _actions.State == PlayerState.InGame;

    private void OnMove(InputValue value)
    {
        if (InGame) _actions.Move(value.Get<Vector2>());
    }

    private void OnJump()
    {
        if (InGame) _actions.Jump();
    }

    private void OnSprint(InputValue value)
    {
        _actions.Sprint(value.isPressed);
    }

    private void OnCrouch(InputValue value)
    {
        _actions.Crouch(value.isPressed);
    }

    private void OnLook(InputValue value)
    {
        if (InGame) _actions.Look(value.Get<Vector2>() * _mouseSensitivity);
    }

    private void OnTogglePerspective()
    {
        if (InGame) _actions.TogglePerspective();
    }

    private void OnAttack()
    {
        if (InGame) _actions.Attack();
    }

    private void OnInteract()
    {
        if (InGame) _actions.Interact();
    }

    private void OnPauseGame()
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
                _actions.OpenPauseMenu();
                break;
            case PlayerState.InPauseMenu:
            case PlayerState.InInventory:
                _actions.CloseAnyMenu();
                break;
        }
    }

    private void OnOpenInventory()
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
                _actions.OpenInventory();
                break;
            case PlayerState.InInventory:
                _actions.CloseAnyMenu();
                break;
        }
    }

    private void OnScrollItem(InputValue value)
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
            case PlayerState.InInventory:
                switch (value.Get<float>())
                {
                    case 0:
                        return;
                    case < 0 when !_reverseScrollDirection:
                    case > 0 when _reverseScrollDirection:
                        _actions.NextItem();
                        break;
                    default:
                        _actions.PreviousItem();
                        break;
                }
                break;
        }
    }

    private void OnNextItem()
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
            case PlayerState.InInventory:
                _actions.NextItem();
                break;
        }
    }

    private void OnPreviousItem()
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
            case PlayerState.InInventory:
                _actions.PreviousItem();
                break;
        }
    }

    private void OnItem1() => SelectItemSlot(1);
    private void OnItem2() => SelectItemSlot(2);
    private void OnItem3() => SelectItemSlot(3);
    private void OnItem4() => SelectItemSlot(4);
    private void OnItem5() => SelectItemSlot(5);
    private void OnItem6() => SelectItemSlot(6);
    private void OnItem7() => SelectItemSlot(7);
    private void OnItem8() => SelectItemSlot(8);

    private void SelectItemSlot(int itemSlot)
    {
        switch (_actions.State)
        {
            case PlayerState.InGame:
            case PlayerState.InInventory:
                _actions.SelectItem(itemSlot - 1);
                break;
        }
    }
}