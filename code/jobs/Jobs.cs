using System;
using System.Collections.Generic;
using Sandbox;

namespace Cinema.Jobs;

/// <summary>
/// Traits & Ablities that jobs have
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
        },
        /// <summary>
        /// Usher job, can pickup garbage
        /// </summary>
        new JobDetails
        {
            Name = "Usher",
            Abilities = JobAbilities.PickupGarbage,
        }
    };
}


public partial class PlayerJob : EntityComponent<Player>, ISingletonComponent
{
    [Net]
    public JobDetails JobDetails { get; set; }

    public new string Name => JobDetails?.Name ?? "Jobless";

    public JobAbilities Abilities => JobDetails?.Abilities ?? 0;

    public bool HasAbility(JobAbilities ability) => Abilities.HasFlag(ability);

    public static PlayerJob CreateFromDetails(JobDetails details)
    {
        var job = new PlayerJob
        {
            JobDetails = details,
        };

        return job;
    }
}
