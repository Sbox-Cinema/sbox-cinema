using Sandbox;
using Cinema.Jobs;


namespace Cinema;

public partial class PopcornStorage : Machine
{
    public static float PopcornStorageTime => 1.0f;
    public static int PopcornStoragePaymentAmount => 10;

    public override string Name => "Popcorn Storage";

    public override bool TimedUse => true;

    public override string UseText => "Store Popcorn";

    [Net]
    public Player BeingUsedBy { get; private set; }

    [Net]
    public TimeUntil TimeUntilPopcornStored { get; private set; }

    public bool IsStoringPopcorn => BeingUsedBy is not null;

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

    public override void OnStopUse(Entity user)
    {
        if (user is not Player player) return;
        if (BeingUsedBy != player) return;

        BeingUsedBy = null;
    }

    private void StartStoringPopcorn(Player player)
    {
        BeingUsedBy = player;
        TimeUntilPopcornStored = PopcornStorageTime;
    }

    private void FinishStoringPopcorn()
    {
        // Remove the popcorn the player is holding
        BeingUsedBy.Inventory.RemoveWeapon(BeingUsedBy.ActiveChild, false);
        BeingUsedBy.AddMoney(PopcornStoragePaymentAmount);
        BeingUsedBy = null;
    }
}
