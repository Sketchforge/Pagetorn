using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Handles ALL Input from the Unity Input System (Old or New)

    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";
    private const string Crouch = "Crouch";
    private const string Sprint = "Sprint";
    private const string Cancel = "Cancel";

    // Sends static Actions / Events that other entities (Player, Game Manager, UI, etc.) respond to

    public static Action OnPause = delegate { };

    // Alternatively just contains all functions that refer to the Input System, such as below

    public static float GetHorizontalAxis(bool raw = false) => raw ? Input.GetAxisRaw(Horizontal) : Input.GetAxis(Horizontal);
    public static float GetVerticalAxis(bool raw = false) => raw ? Input.GetAxisRaw(Vertical) : Input.GetAxis(Vertical);
    public static bool GetCrouching() => Input.GetButton(Crouch);
    public static bool GetSprinting() => Input.GetButton(Sprint);

    private void Update()
    {
        if (Input.GetButtonDown(Cancel))
        {
            OnPause?.Invoke();
        }
    }
}