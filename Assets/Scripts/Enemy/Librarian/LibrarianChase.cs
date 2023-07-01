using UnityEngine;
using System.Collections.Generic;

public class LibrarianChase : EnemyBase
{
    [SerializeField] GameObject librarian;

    protected override Targetable GetPotentialTarget(IEnumerable<Targetable> potentialTargets)
    {
        Targetable target = null;
        int targetPriority = -1;
        foreach (Targetable t in potentialTargets)
        {

            if (targetPriority < 5 && t.Type == TargetableType.Player)
            {
                target = t;
                targetPriority = 5;
            }

        }
        return target;
    }

    protected override void OnLoseTarget()
    {
        if (librarian) Instantiate(librarian, this.transform, true);
        //Destroy(this.gameObject);
    }

    protected override void OnAwake()
    {
    }

    protected override void OnStart()
    {
        Debug.Log("Spawned Evil Guy");
        CheckTarget();
    }

    protected override void OnUpdate()
    {
        if (CheckTarget())
        {
            if (_target.Type == TargetableType.Player)
                MoveTo(_target.transform.position + new Vector3(5 / 2, 0, 5 / 2));
        }
    }
}
