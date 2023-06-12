using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Cinema.Jobs;

/// <summary>
/// Traits & Abilities that jobs have
/// </summary>
[Flags]
public enum JobAbilities : ulong
{
    // Is this job a guest (not worker)
    Guest = 1 << 0,
    // Can this job purchase concessions
    PurchaseConcessions = 1 << 1,
    // Can this job pickup garbage
    PickupGarbage = 1 << 2,
    // Can this job make popcorn
    MakePopcorn = 1 << 3
}

public partial class JobDetails : BaseNetworkable
{
    /// <summary>
    /// The name of this job
    /// </summary>
    [Net]
    public string Name { get; set; }

    /// <summary>
    /// What abilities this job has
    /// </summary>
    [Net]
    public JobAbilities Abilities { get; set; }

    /// <summary>
    /// What responsibilities this job has
    /// </summary>
    [Net]
    public JobResponsibilities Responsibilities { get; set; }

    /// <summary>
    /// The set of clothing that will automatically be applied to any player
    /// who selects this job. If null, the player's avatar clothing will be use.
    /// </summary>
    [Net]
    public string Uniform { get; set; }

    public static JobDetails DefaultJob => All[0];

    public static List<JobDetails> All => new()
    {
        /// <summary>
        /// The default job for players, is a guest
        /// </summary>
        new JobDetails
        {
            Name = "Guest",
            Abilities = JobAbilities.Guest | JobAbilities.PurchaseConcessions,
            Responsibilities = JobResponsibilities.UniversalIncome,
        },
        /// <summary>
        /// Usher job, can pickup garbage
        /// </summary>
        new JobDetails
        {
            Name = "Usher",
            Abilities = JobAbilities.PickupGarbage,
            Responsibilities = 0,
            Uniform = "usher"
        },
        /// <summary>
        /// Concession worker who can make and store popcorn
        /// </summary>
        new JobDetails {
            Name = "Concession Worker",
            Abilities = JobAbilities.MakePopcorn,
            Responsibilities = JobResponsibilities.PopcornStocking,
            Uniform = "usher"
        }
    };
}


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
