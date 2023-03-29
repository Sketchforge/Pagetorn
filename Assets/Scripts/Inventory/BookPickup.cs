using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookPickup : MoveTowards
{

    // Start is called before the first frame update
    public override void ReachedTargetAction()
    {
        _playerTarget.GetComponentInParent<PlayerManager>()._playerInventory.AddItemToInventory(_objectIntoInventory, 1);
        Destroy(gameObject);
    }
}
