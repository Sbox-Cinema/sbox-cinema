using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Cinema;

public partial class PlayerInventory : EntityComponent<Player>, ISingletonComponent
{
    [Net] public IList<Carriable> Weapons { get; protected set; }

    /// <summary>
    /// Adds a weapon to the player's inventory.
    /// </summary>
    /// <param name="weapon">The weapon to add</param>
    /// <param name="makeActive">Whether the weapon should become active right away</param>
    /// <returns>Whether the weapon was successfully added</returns>
    public bool AddWeapon(Carriable weapon, bool makeActive = true)
    {
        if (weapon.Parent.IsValid()) return false;
        if (Weapons.Contains(weapon)) return false;

        Weapons.Add(weapon);
        weapon.OnCarryStart(Entity);

        if (makeActive || Weapons.Count == 1)
            Entity.ActiveChild = weapon;

        return true;
    }

    /// <summary>
    /// Removes a weapon from the players inventory.
    /// </summary>
    /// <param name="weapon">The weapon to remove</param>
    /// <param name="drop">Whether the weapon should be dropped on the ground</param>
    /// <returns>If the weapon was removed</returns>
    public bool RemoveWeapon(Carriable weapon, bool drop = true)
    {
        var success = Weapons.Remove(weapon);
        if (!success) return false;

        if (!drop)
        {
            weapon.Delete();
            return true;
        }

        weapon.OnCarryDrop(Entity);
        // Apply some force to the weapon
        weapon.PhysicsGroup.Velocity = Entity.Velocity + Entity.AimRay.Forward * 100;

        if (Entity.ActiveChild == weapon)
            Entity.ActiveChild = GetBestWeapon();

        return true;
    }

    /// <summary>
    /// Finds the best weapon in the player's inventory.
    /// </summary>
    /// <returns>The best weapon</returns>
    public Carriable GetBestWeapon()
    {
        return Weapons.ToList().FirstOrDefault();
    }

    protected override void OnDeactivate()
    {
        if (Game.IsServer)
        {
            Weapons.ToList()
                .ForEach(x => x.Delete());
        }
    }
}
