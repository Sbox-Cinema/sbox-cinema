using System.Collections.Generic;

namespace Cinema.Jobs;

public partial class JobDetails
{
    public static JobDetails DefaultJob => All[0];

    public static List<JobDetails> All => new()
    {
        /// <summary>
        /// The default job for players, is a guest
        /// </summary>
        new JobDetails
        {
            Name = "Guest",
            Description = "A guest of the cinema",
            LongDescription = "Sit back and enjoy videos.\nBuy concessions and enjoy the show!",
            Abilities = JobAbilities.Guest | JobAbilities.PurchaseConcessions,
            Responsibilities = JobResponsibilities.UniversalIncome,
        },
        /// <summary>
        /// Usher job, can pickup garbage
        /// </summary>
        new JobDetails
        {
            Name = "Usher",
            Description = "Cleans up the cinema",
            LongDescription = "Clean up garbage and keep the cinema clean!\nEarn money by picking up and throwing away garbage.",
            Abilities = JobAbilities.PickupGarbage,
            Responsibilities = 0,
            Uniform = "usher",
            Items = new[]{"trash_bag"}
        },
        /// <summary>
        /// Concession worker who can make and store popcorn
        /// </summary>
        new JobDetails {
            Name = "Concession Worker",
            Description = "Keep the concessions stocked",
            LongDescription = "Stock the concession stand with popcorn, hotdogs, nachos, and soda!\nEarn money by completing tasks.",
            Abilities = JobAbilities.MakePopcorn,
            Responsibilities = JobResponsibilities.PopcornStocking,
            Uniform = "usher"
        }
    };

}
