using Sandbox;

namespace Cinema.Jobs;

public partial class PlayerJob : EntityComponent<Player>, ISingletonComponent
{
    [Net]
    public JobDetails JobDetails { get; set; }

    public new string Name => JobDetails?.Name ?? "Jobless";

    public JobAbilities Abilities => JobDetails?.Abilities ?? 0;

    public JobResponsibilities Responsibilities => JobDetails?.Responsibilities ?? 0;

    public bool HasAbility(JobAbilities ability) => Abilities.HasFlag(ability);

    public bool HasResponsibility(JobResponsibilities responsibility) => Responsibilities.HasFlag(responsibility);

    public static PlayerJob CreateFromDetails(JobDetails details)
    {
        var job = new PlayerJob
        {
            JobDetails = details,
        };

        return job;
    }

    protected override void OnActivate()
    {
        base.OnActivate();

        // It is assumed that whenever this component is activated on a player, their job
        // is being changed to the job described by this component.
        Event.Run(CinemaEvent.JobChanged, Entity);
    }
}
