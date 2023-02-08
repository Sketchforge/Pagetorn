using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class AIManager : MonoBehaviour
{
    private static AIManager _instance;

    public static AIManager Instance
    {
        get
        {
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public Transform _target;
    public float _staticRadiusAroundTarget = 0.5f;
    public List<EnemyBase> Units = new List<EnemyBase>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20,20,200,50), "Move to Target"))
        {
            makeAgentsCircleTarget();
        }
    }

    private void makeAgentsCircleTarget()
    {
        for (int i = 0; i < Units.Count; i++)
        {
            Units[i].MoveTo(new Vector3(
                _target.position.x + _staticRadiusAroundTarget * Mathf.Cos(2 * Mathf.PI * i / Units.Count),
                _target.position.y,
                _target.position.z + _staticRadiusAroundTarget * Mathf.Sin(2 * Mathf.PI * i / Units.Count)));
        }
    }
}
