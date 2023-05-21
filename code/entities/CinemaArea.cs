using Sandbox;

namespace Cinema;

public partial class CinemaArea : BaseTrigger
{
    /// <summary>
    /// The projector entity that will play in this area
    /// </summary>
    [Property]
    public EntityTarget Projector { get; set; }

    public ProjectorEntity ProjectorEntity => Projector.GetTarget<ProjectorEntity>();
}
