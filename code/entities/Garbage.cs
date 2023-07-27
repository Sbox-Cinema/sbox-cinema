using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Cinema;

/// <summary>
/// A piece of garbage that can be picked up and thrown away.
/// </summary>
public partial class Garbage : AnimatedEntity, ICinemaUse
{
    public static List<Garbage> AllGarbage { get; set; } = new();
    public static int MaxGarbage { get; set; } = 30;
    new public virtual string Name => "Garbage";
    public virtual string ModelPath { get; set; } = "models/sbox_props/bin/rubbish_bag.vmdl_c";
    public string UseText => "Pickup Garbage";
    public string CannotUseText => GetPlayerGarbageBag(LocalPlayer).RemainingSpace <= 0 ? "Garbage bag is full" : "Cannot pickup";
    public bool ShowCannotUsePrompt => LocalPlayer.Job.HasAbility(Jobs.JobAbilities.PickupGarbage);
    public string TypeOfGarbage => ModelPath;
    private static Player LocalPlayer => Game.LocalPawn as Player;

    public override void Spawn()
    {
        base.Spawn();

        if (TooMuchGarbage()) RemoveOldGarbage();

        AllGarbage.Add(this);

        if (ModelPath != null)
        {
            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic, false);
        }

        Tags.Add("garbage");
    }

    public bool IsUsable(Entity user)
    {
        if (user is not Player ply)
            return false;

        if (!ply.Job.HasAbility(Jobs.JobAbilities.PickupGarbage)) return false;

        var bag = GetPlayerGarbageBag(ply);
        if (bag == null) return false;
        if (bag.RemainingSpace <= 0) return false;

        return true;
    }

    public bool OnUse(Entity user)
    {
        if (user is not Player ply)
            return false;

        var bag = GetPlayerGarbageBag(ply);
        var wasPickedUp = bag.Add(TypeOfGarbage);

        if (wasPickedUp)
            Delete();

        return false;
    }

    public bool OnStopUse(Entity user)
    {
        return true;
    }

    public static GarbageBag GetPlayerGarbageBag(Player player)
    {
        var trashBag = player.Inventory.FindItems<GarbageBag>().FirstOrDefault();
        return trashBag;
    }

    public static bool TooMuchGarbage()
    {
        AllGarbage.RemoveAll(x => !x.IsValid());
        return AllGarbage.Count >= MaxGarbage;
    }

    public static void RemoveOldGarbage()
    {
        AllGarbage.FirstOrDefault()?.Delete();
    }
}
