
using System;
using Sandbox;

namespace Cinema.Jobs;

/// <summary>
/// Responsibilities that jobs have.
/// If an EntityComponent that derives from `JobResponsibility` exists
/// With the **exact** same name as the enum, it will be added to the player.
/// </summary>
[Flags]
public enum JobResponsibilities : ulong
{
    // Gives you money for playing on the server
    UniversalIncome = 1 << 0,
}

public partial class JobResponsibility : EntityComponent<Player>
{
    public new virtual string Name => "Responsibility";
}

public partial class UniversalIncome : JobResponsibility
{
    public override string Name => "Universal Income";

    private static int IncomeAmount => 50;
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
