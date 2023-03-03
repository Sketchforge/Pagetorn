[System.Serializable]
public struct ItemAmount
{
    public Item Item;
    public int Amount;

    public ItemAmount(int amount = 1, Item item = null)
    {
        Item = item;
        Amount = amount;
    }
}