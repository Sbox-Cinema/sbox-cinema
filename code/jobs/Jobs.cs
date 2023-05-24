using System;
using System.Collections.Generic;
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
    /// The interval for a job fail to be forgiven
    /// </summary>
    public float ForgiveInterval;

    /// <summary>
    /// The forgiveness timer
    /// </summary>
    public TimeUntil ForgiveTimer;

    /// <summary>
    /// How many fails are allowed on the job before being fired, set -1 if employees cannot be fired for fails
    /// </summary>
    public int FailAllowance;

    /// <summary>
    /// The fails in total
    /// </summary>
    [Net]
    public int Fails { get; set; } = 0;

    /// <summary>
    /// What responsibilities this job has
    /// </summary>
    [Net]
    public JobResponsibilities Responsibilities { get; set; }

    /// <summary>
    /// When the player leaves, apply the job cooldown so they can't be assigned to another job
    /// </summary>
    public float BaseLeaveCooldown;

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
            FailAllowance = -1,
            BaseLeaveCooldown = -1,
            Responsibilities = JobResponsibilities.UniversalIncome,
        },

        /// <summary>
        /// Usher job, can pickup garbage
        /// </summary>
        new JobDetails
        {
            Name = "Usher",
            Abilities = JobAbilities.PickupGarbage,
            FailAllowance = -1,
            BaseLeaveCooldown = 15,
            Responsibilities = JobResponsibilities.PerTaskIncome,
        },    
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

    public static PlayerJob CreateFromDetails(JobDetails details)
    {
        var job = new PlayerJob
        {
            JobDetails = details,
        };

        return job;
    }

    /// <summary>
    /// Gets the default job
    /// </summary>
    /// <returns>The default job</returns>
    public static PlayerJob GetDefaultJob()
    {
        var defJob = new PlayerJob
        {
            JobDetails = JobDetails.DefaultJob,
        };

        return defJob;
    }
}
