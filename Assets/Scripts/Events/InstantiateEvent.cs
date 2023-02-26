using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pagetorn/Events/InstantiateEvent")]
public class InstantiateEvent : Event
{
    [Header("Instantiate")]
    [SerializeField] private GameObject _objToSpawn;
    [SerializeField] private Vector3 _offsetFromPlayer = Vector3.forward * 2 + Vector3.up * 0.5f;
    [SerializeField] private float _randomDistFromPlayerAndOffset = 1;
    [SerializeField] private int _instantiateCount = 1;
    [SerializeField] private bool _facePlayer = true;

    public override void ActivateEvent()
    {
        Transform player = PlayerManager.Instance.Player;
        var pos = player.TransformPoint(_offsetFromPlayer);
        for (int i = 0; i < _instantiateCount; i++)
        {
            var offset = Random.insideUnitSphere * _randomDistFromPlayerAndOffset;
            offset.y = 0;
            Debug.Log(pos + " " + offset);
            var obj = Instantiate(_objToSpawn, pos + offset, Quaternion.identity);
            if (_facePlayer)
            {
                obj.transform.LookAt(player, Vector3.up);
            }
        }
    }
}