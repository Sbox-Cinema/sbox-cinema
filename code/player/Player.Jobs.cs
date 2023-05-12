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
        Components.Add(Jobs.PlayerJob.CreateFromDetails(details));
    }

    [ConCmd.Server("cinema.job.set")]
    public static void SetJob(int job = 0)
    {
        var player = ConsoleSystem.Caller.Pawn as Player;
        if ( player == null ) return;

        if (job < 0) return;
        if (job > Jobs.JobDetails.All.Count-1) return;

        player.SetJob(Jobs.JobDetails.All[job]);

        Log.Info($"Assigned to '{player.Job.Name}'");
    }
}
