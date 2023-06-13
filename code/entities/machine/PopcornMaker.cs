using System;
using Sandbox;
using Cinema.Jobs;


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

    public override bool IsUsable(Entity user)
    {
        if (user is not Player player) return false;
        if (IsMakingPopcorn) return BeingUsedBy == player;
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

    public override void OnStopUse(Entity user)
    {
        if (user is not Player player) return;
        if (BeingUsedBy != player) return;

        BeingUsedBy = null;
    }

    private void StartMakingPopcorn(Player player)
    {
        BeingUsedBy = player;
        TimeUntilPopcornFinished = PopcornCreationTime;
    }

    private void FinishMakingPopcorn()
    {
        //BeingUsedBy.Inventory.AddWeapon(new Popcorn(), true);
        BeingUsedBy = null;
    }
}
