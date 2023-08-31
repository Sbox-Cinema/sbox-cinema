using System;
using Sandbox;
using Cinema.Jobs;
using Conna.Inventory;

namespace Cinema;

public partial class PopcornMaker : Machine
{
    public static float PopcornCreationTime => 1.0f;

    public override string Name => "Popcorn Maker";

    public override bool TimedUse => true;

    public override string UseText => "Make Popcorn";

    [Net]
    public Player BeingUsedBy { get; private set; }

    [Net]
    public TimeUntil TimeUntilPopcornFinished { get; private set; }

    public bool IsMakingPopcorn => BeingUsedBy is not null;

    public override float TimedUsePercentage => IsMakingPopcorn ? Math.Min(TimeUntilPopcornFinished.Passed / PopcornCreationTime, 1) : 0;

    public static string PopcornItemId => "popcorn_tub";

    public override bool IsUsable(Entity user)
    {
        if (user is not Player player) return false;
        if (IsMakingPopcorn) return BeingUsedBy == player;
        if (!player.HasInventorySpace) return false;
        return player.Job.HasAbility(JobAbilities.MakePopcorn);
    }

    public override bool OnUse(Entity user)
    {
        if (user is not Player player) return false;

        if (!IsMakingPopcorn)
        {
            StartMakingPopcorn(player);
        }

        if (TimeUntilPopcornFinished.Relative < 0)
        {
            FinishMakingPopcorn();
            return false;
        }

        return true;
    }

    public override bool OnStopUse(Entity user)
    {
        if (user is not Player player) return true;
        if (BeingUsedBy != player) return true;

        BeingUsedBy = null;

        return true;
    }

    private void StartMakingPopcorn(Player player)
    {
        BeingUsedBy = player;
        TimeUntilPopcornFinished = PopcornCreationTime;
    }

    private void FinishMakingPopcorn()
    {
        var player = BeingUsedBy;
        BeingUsedBy = null;
        if (!player.HasInventorySpace) return;

        var item = InventorySystem.CreateItem(PopcornItemId);
        player.PickupItem(item);

    }
}
