using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SimplePlayerTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPlayerEnter;
    [SerializeField] private UnityEvent _onPlayerLeave;
    [SerializeField, ReadOnly] private bool _playerInside;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_playerInside && other.GetComponent<PlayerMovementScript>())
        {
            _playerInside = true;
            _onPlayerEnter.Invoke();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (_playerInside && other.GetComponent<PlayerMovementScript>())
        {
            _playerInside = false;
            _onPlayerLeave.Invoke();
        }
    }
}
