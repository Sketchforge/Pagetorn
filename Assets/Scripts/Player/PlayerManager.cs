using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Handles Player Death, Survival Stats, Special Events, etc. Serves mainly as a reference point for player-related components

    public static PlayerManager Instance;

    [SerializeField] private Transform _player;
    [SerializeField] private PlayerMovementScript _movementScript;
    [SerializeField] private PlayerActions _playerActions;

    [SerializeField, DrawSO] private SurvivalStat _health;
    [SerializeField, DrawSO] private SurvivalStat _hunger;
    [SerializeField, DrawSO] private SurvivalStat _hydration;

    [SerializeField, DrawSO] private Inventory _inventory;
    [SerializeField, DrawSO] private Magic _magic;

    [SerializeField] private Transform _respawnPoint;
    [SerializeField, ReadOnly] private bool _isDead;

    public Transform Player => _player;
    public bool Dead => _isDead;

    #region Unity Functions

    private void Awake()
    {
        // Although this is a Singleton, it should not be DontDestroyOnLoad because it is a player living only in this scene
        Instance = this;
    }

    private void OnEnable()
    {
        _health.OnReachZero += KillPlayer;
        _hunger.OnReachZero += KillPlayer;
        _hydration.OnReachZero += KillPlayer;
    }

    private void OnDisable()
    {
        _health.OnReachZero -= KillPlayer;
        _hunger.OnReachZero -= KillPlayer;
        _hydration.OnReachZero -= KillPlayer;
    }

    private void OnValidate()
    {
        if (_movementScript == null)
        {
            _movementScript = GetComponent<PlayerMovementScript>();
            if (_movementScript == null)
            {
                _movementScript = GetComponentInChildren<PlayerMovementScript>();
                if (_movementScript == null)
                {
                    _movementScript = FindObjectOfType<PlayerMovementScript>();
                }
            }
        }
        if (_player == null)
        {
            _player = transform.Find("Player");
            if (_player == null) _player = _movementScript ? _movementScript.transform : transform;
        }
        if (_playerActions == null)
        {
            _playerActions = GetComponent<PlayerActions>();
            if (_playerActions == null)
            {
                _playerActions = GetComponentInChildren<PlayerActions>();
                if (_playerActions == null)
                {
                    _playerActions = FindObjectOfType<PlayerActions>();
                }
            }
        }
    }

    #endregion

    [Button(Mode = ButtonMode.InPlayMode)]
    public void KillPlayer()
    {
        _isDead = true;
        _movementScript.OnPlayerDeath();
    }

    [Button(Mode = ButtonMode.InPlayMode)]
    public void RespawnPlayer()
    {
        _isDead = false;
        var pos = _respawnPoint ? _respawnPoint.position : Vector3.zero;
        var rot = _respawnPoint ? _respawnPoint.rotation : Quaternion.identity;
        Player.SetPositionAndRotation(pos, rot);
        _movementScript.OnPlayerRespawn();
        ResetPlayer();
    }

    private void Start()
    {
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        _health.SetToMax();
        _hunger.SetToMax();
        _hydration.SetToMax();
    }
}