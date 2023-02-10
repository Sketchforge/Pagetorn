using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private Event _event;
    [SerializeField] private bool _destroySelf;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovementScript>())
        {
            _event.ActivateEvent(other.transform);
            if (_destroySelf) Destroy(gameObject);
        }
    }
}
