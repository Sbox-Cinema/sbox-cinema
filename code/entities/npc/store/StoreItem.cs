using Conna.Inventory;
using Sandbox;
using System;

namespace Cinema;

public class StoreItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Cost { get; set; }
    public string Icon { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description, Cost, Icon);
    }

    /// <summary>
    /// Not available client side
    /// </summary>
    public Action<Player> OnPurchase { get; set; }

    public StoreItem GivesItem(string itemUniqueId)
    {
        OnPurchase = (Player player) =>
        {
            var item = InventorySystem.CreateItem(itemUniqueId);
            if (!item.IsValid())
            {
                Log.Error($"Failed to create item with id of {itemUniqueId}");
                return;
            }

            player.PickupItem(item);
        };

        return this;
    }

    public static StoreItem Item(string name, string description, string icon, int cost, string itemUniqueId)
    {
        var item = new StoreItem()
        {
            Name = name,
            Description = description,
            Icon = icon,
            Cost = cost
        };

        item.GivesItem(itemUniqueId);

        return item;
    }
}
