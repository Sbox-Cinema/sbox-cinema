using Sandbox;

namespace Cinema;

public abstract class BaseNeed : EntityComponent<Player>, INeedInfo
{
    [ConVar.Replicated("player.needs.globaldecayfactor")]
    public static float NeedDecayFactor { get; set; } = 1f;

    public abstract string DisplayName { get; }
    public abstract string IconPath { get; }
    public abstract float SatisfactionPercent { get; }
    
}
