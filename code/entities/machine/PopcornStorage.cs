using System.Linq;
using Sandbox;
using Cinema.Jobs;
using System;

namespace Cinema;

public partial class PopcornStorage : Machine
{
    public static float PopcornStorageTime => 1.0f;
    public static RangedFloat PopcornDecayTime => new(5f, 10f);
    public static int PopcornStoragePaymentAmount => 10;
    public static int PopcornOutOfStockPenaltyAmount => 50;

    public override string Name => "Popcorn Storage";

    public override bool TimedUse => true;

    public override string UseText => "Store Popcorn";

    [Net]
    public Player BeingUsedBy { get; private set; }

    [Net]
    public TimeUntil TimeUntilPopcornStored { get; private set; }

    [Net]
    public TimeUntil TimeUntilPopcornDecays { get; private set; } = PopcornDecayTime.GetValue();

    [Net]
    public int PopcornStored { get; private set; } = 0;

    public bool IsStoringPopcorn => BeingUsedBy is not null;

    public override float TimedUsePercentage => IsStoringPopcorn ? Math.Min(TimeUntilPopcornStored.Passed / PopcornStorageTime, 1) : 0;

    public override bool IsUsable(Entity user)
    {
        if (user is not Player player) return false;
        if (IsStoringPopcorn && BeingUsedBy != player) return false;
        if (!player.Job.HasAbility(JobAbilities.MakePopcorn)) return false;

        return player.ActiveChild is Popcorn;
    }

    public override bool OnUse(Entity user)
    {
        if (user is not Player player) return false;

        if (!IsStoringPopcorn)
        {
            StartStoringPopcorn(player);
        }

        if (TimeUntilPopcornStored < 0)
        {
            FinishStoringPopcorn();
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

    [GameEvent.Tick.Server]
    private void Tick()
    {
        HandlePopcornDecay();
    }

    private void HandlePopcornDecay()
    {
        if (TimeUntilPopcornDecays.Relative > 0) return;

        if (PopcornStored > 0)
        {
            --PopcornStored;
        }
        else
        {
            OnNoPopcornLeft();
        }

        TimeUntilPopcornDecays = PopcornDecayTime.GetValue();
    }

    private static void OnNoPopcornLeft()
    {
        var stockers = All.OfType<Player>().Where(p => p.Job.HasResponsibility(JobResponsibilities.PopcornStocking));
        foreach (var stocker in stockers)
        {
            stocker.TakeMoney(PopcornOutOfStockPenaltyAmount);
        }
    }

    private void StartStoringPopcorn(Player player)
    {
        BeingUsedBy = player;
        TimeUntilPopcornStored = PopcornStorageTime;
    }

    private void FinishStoringPopcorn()
    {
        // Remove the popcorn the player is holding
        BeingUsedBy.Inventory.Remove(BeingUsedBy.ActiveChild.Item);
        BeingUsedBy.AddMoney(PopcornStoragePaymentAmount);
        ++PopcornStored;
        BeingUsedBy = null;
    }
}
