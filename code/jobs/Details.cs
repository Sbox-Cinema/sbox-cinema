using System;
using System.Collections.Generic;
using Sandbox;

namespace Cinema.Jobs;

public partial class JobDetails : BaseNetworkable, IEquatable<JobDetails>
{
    /// <summary>
    /// The name of this job
    /// </summary>
    [Net]
    public string Name { get; set; }

    /// <summary>
    /// The short description of the job
    /// </summary>
    public string Description { get; set; }


    /// <summary>
    /// The long description of the job
    /// </summary>
    public string LongDescription { get; set; }

    /// <summary>
    /// What abilities this job has
    /// </summary>
    [Net]
    public JobAbilities Abilities { get; set; }

    /// <summary>
    /// Unique ID's of items that this job should have
    /// </summary>
    public string[] Items { get; set; }

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

    public bool Equals(JobDetails other)
    {
        return other.Name == Name;
    }

    public override bool Equals(object other)
    {
        return other is JobDetails job && Equals(job);
    }

    public static bool operator ==(JobDetails a, JobDetails b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null) return false;
        if (b is null) return false;
        return a.Equals(b);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public static bool operator !=(JobDetails a, JobDetails b) => !(a == b);
}
