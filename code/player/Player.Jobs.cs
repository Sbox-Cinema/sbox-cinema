using Sandbox;

namespace Cinema;

public partial class Player
{
    [BindComponent]
    public Jobs.PlayerJob Job { get; }

    [ConCmd.Server("cinema.jobtest")]
    public static void OnReload()
    {
        var ply = ConsoleSystem.Caller.Pawn as Player;
        //ply.Components.Add(Jobs.PlayerJob.CreateFromDetails(Jobs.JobDetails.All.First()));
    }

    public void SetJob(Jobs.JobDetails details)
    {
        if (Game.IsClient) throw new System.Exception("Cannot set job on client!");

        Components.RemoveAny<Jobs.PlayerJob>();
        Components.Add(Jobs.PlayerJob.CreateFromDetails(details));
    }
}
