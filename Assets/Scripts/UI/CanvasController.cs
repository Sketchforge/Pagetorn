using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private void OnEnable()
    {
        GameManger.OnPause += OnPause;
    }

    private void OnDisable()
    {
        GameManger.OnPause -= OnPause;
    }

    private void OnPause(bool paused)
    {
        _pauseMenu.SetActive(paused);
    }
}