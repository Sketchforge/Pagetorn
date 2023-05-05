using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Pagetorn/Events/InstantiateEvent")]
public class InstantiateEvent : Event
{
    [Header("Instantiate")]
    [SerializeField] private GameObject _objToSpawn;
    [SerializeField] private Vector3 _offsetFromPlayer = Vector3.forward * 2 + Vector3.up * 0.5f;
    [SerializeField] private float _randomDistFromPlayerAndOffset = 1;
    [SerializeField] private int _instantiateCount = 1;
    [SerializeField] private bool _facePlayer = true;

    public override void ActivateEvent(System.Action onFinished = null)
    {
        Transform player = PlayerManager.Instance.Player;
        var pos = player.TransformPoint(_offsetFromPlayer);
        for (int i = 0; i < _instantiateCount; i++)
        {

            if (RandomPoint(player.position, _randomDistFromPlayerAndOffset, out var point))
            {
                var obj = Instantiate(_objToSpawn, point, Quaternion.identity);
                if (_facePlayer)
                {
                    obj.transform.LookAt(player, Vector3.up);
                }
            }
            
        }
    }


    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
    for (int i = 0; i < 30; i++)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
    }
    result = Vector3.zero;
    return false;
    }
 
}