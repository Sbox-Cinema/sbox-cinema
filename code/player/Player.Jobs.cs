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
}
