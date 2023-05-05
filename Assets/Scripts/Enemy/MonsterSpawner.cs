using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<InstantiateEvent> monsterEvents;
    [SerializeField] public int _indexMonstersToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] public float _playerCheckRange;
    private float _myTimer = DataReactor.monsterSpawnRate;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = this.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        var targets = Targetable.GetAllWithinRange(transform.position, _playerCheckRange);
        foreach (Targetable entity in targets)
        {
            PlayerMovementScript player = entity.GetComponent<PlayerMovementScript>();
            if (player != null)
            {
                //found player
                if (_myTimer <= 0)
                {
                    for (int i = 0; i <= _indexMonstersToSpawn; i++)
                    {
                        monsterEvents[i].ActivateEvent();
                    }
                    spawnPoint.position += new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                    _myTimer = DataReactor.monsterSpawnRate;
                }
            }
        }
        
        _myTimer -= Time.deltaTime;
        
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(this.gameObject.transform.position, transform.up, _playerCheckRange);
    }
#endif
}
