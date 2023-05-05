using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[DefaultExecutionOrder(1)]

public class AIUnit : MonoBehaviour
{
    public NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        
    }

    public virtual void MoveTo(Vector3 target)
    {
        Agent.SetDestination(target);
    }
}
