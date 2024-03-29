using System;
using CoffeyUtils;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static Action<bool> OnPause = delegate { };

    public static GameData Data => Instance._data;
    
    [SerializeField] private GameData _data;
    [SerializeField, ReadOnly] private bool _paused = false;
    [SerializeField] private PostProcessingManager _postProcessingManager;

    public static PostProcessingManager PostProcessingManager => Instance._postProcessingManager;

    private void Awake()
    {
        Instance = this;
        _data.ResetAll();
    }

    private void OnDestroy()
    {
        _data.ResetAll();
    }

    private void Start()
    {
        SetPaused(false);
    }

    // Handle Game Pausing
    // Every script that needs to freeze during game paused has its own implementation referring to the static Action OnPause
    // This allows animation, particles, and minor camera movement to still occur even though the game is paused
    // Time.timeScale is another option, but does not allow for any of these

    [Button(Mode = RuntimeMode.OnlyPlaying)]
    public void TogglePaused()
    {
        SetPaused(!_paused);
    }

    public void SetPaused(bool paused)
    {
        _paused = paused;
        OnPause?.Invoke(paused);
    }
}