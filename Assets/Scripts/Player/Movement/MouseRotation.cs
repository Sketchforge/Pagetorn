using UnityEngine;

public class MouseRotation : MonoBehaviour
{
    public Vector2 turn;
    public float xSensitivity = .5f;
    public float ySensitivity = .5f;
    public Vector3 deltaMove;
    public float speed = 1;

    public bool rotateX = true;
    public bool rotateY = true;

    private bool gamePaused;

    private void OnEnable()
    {
        GameManger.OnPause += OnPause;
    }

    private void OnDisable()
    {
        GameManger.OnPause -= OnPause;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        turn.y = Mathf.Clamp(turn.y, -90, 90);
    }

    void Update()
    {
        if (gamePaused) return;

        if (rotateX)
            turn.x += Input.GetAxis("Mouse X") * xSensitivity;
        if (rotateY)
        {
            turn.y += Input.GetAxis("Mouse Y") * ySensitivity;
            turn.y = Mathf.Clamp(turn.y, -90, 90);
        }
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }

    private void OnPause(bool paused)
    {
        gamePaused = paused;
    }
}