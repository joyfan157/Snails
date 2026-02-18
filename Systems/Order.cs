using Snails.Entities.Items;

namespace Snails.Systems;

public class Order
{
    public ItemType RequestedItem { get; }
    public float TimeRemaining { get; set; }
    public float TotalTime { get; }

    public Order(ItemType requestedItem, float totalTime)
    {
        RequestedItem = requestedItem;
        TotalTime = totalTime;
        TimeRemaining = totalTime;
    }
}
