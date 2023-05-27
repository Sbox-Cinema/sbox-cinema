
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
    // Responsible for keeping popcorn machines stocked
    PopcornStocking = 1 << 1,
}

public partial class JobResponsibility : EntityComponent<Player>
{
    public new virtual string Name => "Responsibility";
}
