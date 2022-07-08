using System;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance;

    public static Action<bool> OnPause = delegate { };

    [SerializeField, ReadOnly] private bool _paused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InputManager.OnPause += TogglePaused;
        SetPaused(false);
    }

    // Handle Game Pausing
    // Every script that needs to freeze during game paused has its own implementation referring to the static Action OnPause
    // This allows animation, particles, and minor camera movement to still occur even though the game is paused
    // Time.timeScale is another option, but does not allow for any of these

    [Button(Mode = ButtonMode.InPlayMode)]
    public void TogglePaused()
    {
        SetPaused(!_paused);
    }

    private void SetPaused(bool paused)
    {
        _paused = paused;
        OnPause?.Invoke(paused);
    }
}