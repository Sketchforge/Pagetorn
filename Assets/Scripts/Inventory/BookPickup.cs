using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPickup : MoveTowards
{
    // Start is called before the first frame update
    public override void ReachedTargetAction()
    {
        Debug.Log("Add to Inventory");
        Destroy(gameObject);
    }
}
