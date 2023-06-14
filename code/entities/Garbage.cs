using System.Linq;
using Sandbox;

namespace Cinema;

/// <summary>
/// A piece of garbage that can be picked up and thrown away.
/// </summary>
public partial class Garbage : AnimatedEntity, ICinemaUse
{
    new public virtual string Name => "Garbage";
    public virtual string ModelPath { get; set; } = "models/sbox_props/bin/rubbish_bag.vmdl_c";
    public string UseText => "Pickup";
    public string TypeOfGarbage { get; set; } = "Garbage";

    public override void Spawn()
    {
        base.Spawn();

        if (ModelPath != null)
        {
            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);
        }
    }

    public bool IsUsable(Entity user)
    {
        if (user is not Player ply)
            return false;

        if (!ply.Job.HasAbility(Jobs.JobAbilities.PickupTrash)) return false;

        var bag = GetPlayerTrashBag(ply);
        if (bag == null) return false;
        if (bag.RemainingSpace <= 0) return false;

        Log.Info("is usable");

        return true;
    }

    public bool OnUse(Entity user)
    {
        if (user is not Player ply)
            return false;

        Log.Info("on use");

        var bag = GetPlayerTrashBag(ply);
        var wasPickedUp = bag.Add(Name);

        if (wasPickedUp)
            Delete();

        return false;
    }

    public void OnStopUse(Entity user)
    {
    }

    public static TrashBag GetPlayerTrashBag(Player player)
    {
        var trashBag = player.Inventory.FindItems<TrashBag>().FirstOrDefault();
        return trashBag;
    }
}
