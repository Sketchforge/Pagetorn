using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowards : MonoBehaviour
{
    [SerializeField] private ScriptableObject _objectIntoInventory;
    [SerializeField] private float _levitateRange = 10f;
    [SerializeField] private float _pickupRange = 1.5f;
    [SerializeField] private float _itemSpeed = 1f;
    [SerializeField, ReadOnly] private Targetable _playerTarget;
    [SerializeField, ReadOnly] private float _distance;
    private float _sinSpeedThresh;
    private float _sinSpeed;

    private void Update()
    {
        _playerTarget = FindPlayer(); // check for and set target (within range)
        if (_playerTarget != null)
        {
            _distance = Vector3.Distance(transform.position, _playerTarget.transform.position);

            _sinSpeedThresh = 1.4f / _levitateRange;
            _sinSpeed = (_sinSpeedThresh * _distance + 3);
            transform.position = Vector3.MoveTowards(transform.position, _playerTarget.transform.position, (_itemSpeed * Mathf.Sin(_sinSpeed) + _itemSpeed) * Time.deltaTime); // fly to player, using a sine function (v*sin(1.4x+3)+v)

            if (_distance < _pickupRange) // check if gloop is in range to be eaten
            {
                ReachedTargetAction();
                
                
            }
        }
    }

    private Targetable FindPlayer()
    {
        var targets = Targetable.GetAllWithinRange(transform.position, _levitateRange);
        foreach (Targetable t in targets)
        {
            if (t.Type == TargetableType.Player)
            {
                return t;
            }
        }
        return null;
    }

    public virtual void ReachedTargetAction()
    {
        //PlayerManager.Instance.GetComponent<Inventory>()
        Debug.Log("Reached Target");
        Destroy(gameObject);
    }
}
