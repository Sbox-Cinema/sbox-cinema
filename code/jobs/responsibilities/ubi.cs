using Sandbox;

namespace Cinema.Jobs;

public partial class UniversalIncome : JobResponsibility
{
    public override string Name => "Universal Income";

    // How much money to give the player every interval
    private static int IncomeAmount => 10;

    // How long until the player gets money again
    private static float IncomeInterval => 60f;

    private TimeUntil TimeUntilNextIncome { get; set; }

    protected override void OnActivate()
    {
        TimeUntilNextIncome = IncomeInterval;
    }

    [GameEvent.Tick.Server]
    protected void Tick()
    {
        if (TimeUntilNextIncome > 0) return;

        TimeUntilNextIncome = IncomeInterval;
        Entity.AddMoney(IncomeAmount);
    }
}
