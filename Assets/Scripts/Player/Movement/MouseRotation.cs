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

    private Vector2 _lookDir;

    private void OnEnable()
    {
        GameManager.OnPause += OnPause;
    }

    private void OnDisable()
    {
        GameManager.OnPause -= OnPause;
    }

    public void SetLookDir(Vector2 lookDir) => _lookDir = lookDir;

    private void Update()
    {
        if (gamePaused) return;

        if (rotateX)
        {
            turn.x += _lookDir.x * xSensitivity;
            turn.x %= 360;
        }
        if (rotateY)
        {
            turn.y += _lookDir.y * ySensitivity;
            turn.y = Mathf.Clamp(turn.y, -90, 90);
        }
        _lookDir = Vector2.zero;
        transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }

    private void OnPause(bool paused)
    {
        gamePaused = paused;
    }
}