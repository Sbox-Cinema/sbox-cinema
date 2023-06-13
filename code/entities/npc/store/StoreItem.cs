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


    public StoreItem GivesWeapon(string weaponClass)
    {
        OnPurchase = (Player player) =>
        {
            Log.Info($"Purchased {weaponClass}");
            var weapon = Entity.CreateByName<Carriable>(weaponClass);
            if (!weapon.IsValid())
            {
                Log.Error($"Failed to create weapon of class {weaponClass}");
                return;
            }

            //player.Inventory.AddWeapon(weapon, true);
        };

        return this;
    }

    public static StoreItem Weapon(string name, string description, string icon, int cost, string weaponClass)
    {
        var item = new StoreItem()
        {
            Name = name,
            Description = description,
            Icon = icon,
            Cost = cost
        };

        item.GivesWeapon(weaponClass);

        return item;
    }
}
