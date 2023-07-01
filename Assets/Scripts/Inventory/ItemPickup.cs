public class ItemPickup : MoveTowards
{

    // Start is called before the first frame update
    public override void ReachedTargetAction()
    {
        _playerTarget.GetComponentInParent<PlayerManager>()._playerInventory.AddItemToInventory(_objectIntoInventory, 1);
        Destroy(gameObject);
    }
}
