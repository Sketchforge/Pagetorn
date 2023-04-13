using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Event : ScriptableObject
{
    [SerializeField] private Event _chainEvent;

    private Event ChainedEvent => _chainEvent;

    public abstract void ActivateEvent(System.Action onFinished = null);

    protected void EventResponse()
    {
        Debug.Log($"Event ({name}) was activated");
        if (_chainEvent) _chainEvent.ActivateEvent(null);
    }

    private void OnValidate()
    {
        var Event = this;
        while (Event && Event.ChainedEvent != null)
        {
            if (Event.ChainedEvent == this)
            {
                _chainEvent = null;
                Debug.LogWarning("Infitinite Loop Detected. Do not chain events to themselves");
                break;
            }
            Event = _chainEvent;
        }
    }
}
