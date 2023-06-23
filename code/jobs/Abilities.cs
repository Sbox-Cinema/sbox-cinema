using System;

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
