using System;
using Sandbox;

namespace Cinema;

public partial class Player
{
    [BindComponent]
    public Jobs.PlayerJob Job { get; }

    public void SetJob(Jobs.JobDetails details)
    {
        if (Game.IsClient) throw new System.Exception("Cannot set job on client!");

        Components.RemoveAny<Jobs.PlayerJob>();

        var currentJob = Job.JobDetails;
        var responsibiltiesToRemove = currentJob.Responsibilities & ~details.Responsibilities;
        // foreach (var res in responsibiltiesToRemove.GetFlags())
        // {
        //     var comp = Components.Remove()
        //     if (comp != null)
        //     {
        //         comp.Remove();
        //     }
        // }

        var responsibiltiesToAdd = details.Responsibilities & ~currentJob.Responsibilities;

        Components.Add(Jobs.PlayerJob.CreateFromDetails(details));
    }
}
