using Sandbox;

namespace Cinema;

public partial class Player
{
    [BindComponent]
    public Jobs.PlayerJob Job { get; }

    public void SetJob(Jobs.JobDetails newJob)
    {
        if (Game.IsClient) throw new System.Exception("Cannot set job on client!");

        UpdateResponsibilities(newJob.Responsibilities);

        Components.RemoveAny<Jobs.PlayerJob>();
        Components.Add(Jobs.PlayerJob.CreateFromDetails(newJob));
    }

    /// <summary>
    /// Updates the responsibility components on the player for a new job.
    /// Will remove any responsibilities that are not in the new job, and add any that are.
    /// </summary>
    /// <param name="newJob">New job responsibilities</param>
    private void UpdateResponsibilities(Jobs.JobResponsibilities newJob)
    {
        var currentJob = Job?.JobDetails.Responsibilities ?? 0;

        var toRemove = currentJob & ~newJob;
        foreach (var res in toRemove.GetFlags())
        {
            var typeDescription = TypeLibrary.GetType("Cinema.Jobs." + res.ToString());
            if (typeDescription is null) continue;

            Components.RemoveAny(typeDescription.TargetType);
        }

        // Filters to only have responsibilities that are in the new job but not the current job
        var toAdd = (newJob ^ currentJob) & newJob;

        foreach (var res in toAdd.GetFlags())
        {
            var typeDescription = TypeLibrary.GetType("Cinema.Jobs." + res.ToString());
            if (typeDescription is null) continue;

            var responsibility = typeDescription.Create<EntityComponent>();
            Components.Add(responsibility);
        }
    }
}
