using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private Event _event;
    [SerializeField] private bool _destroySelf;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovementScript>())
        {
            _event.ActivateEvent();
            if (_destroySelf) Destroy(gameObject);
        }
    }
}
