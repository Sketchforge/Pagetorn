using System.Collections;
using System.Collections.Generic;
using CoffeyUtils;
using UnityEngine;

public class Gloop : MonoBehaviour
{
    [SerializeField] private float _gloopPoints = 5;
    [SerializeField] private float _levitateRange = 10f;
    [SerializeField] private float _pickupRange = 1.5f;
    [SerializeField] private float _gloopSpeed = 1f;
    [SerializeField, ReadOnly] private Targetable _playerTarget;
    [SerializeField, ReadOnly] private float _distance;
    private float _sinSpeedThresh;
    private float _sinSpeed;

    public bool Eaten { get; set; }

    private void Update()
    {
        _playerTarget = FindPlayer(); // check for and set target (within range)
        if (_playerTarget != null)
        {
            _distance = Vector3.Distance(transform.position, _playerTarget.transform.position);

            _sinSpeedThresh = 1.4f / _levitateRange;
            _sinSpeed = (_sinSpeedThresh * _distance + 3);
            transform.position = Vector3.MoveTowards(transform.position, _playerTarget.transform.position, (_gloopSpeed * Mathf.Sin(_sinSpeed) + _gloopSpeed)  * Time.deltaTime); // fly to player, using a sine function (v*sin(1.4x+3)+v)

            if (_distance < _pickupRange) // check if gloop is in range to be eaten
            {
                Eaten = true;
                PlayerManager.Instance.Survival.Increase(SurvivalStatEnum.MagicPoints, _gloopPoints);
                DataManager.NumberKnowledgePointsGathered += _gloopPoints;
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _levitateRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _pickupRange);
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
}
