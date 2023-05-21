using Editor;
using Sandbox;

namespace Cinema;

[Library("cinema_zone"), HammerEntity]
[Title("Cinema Zone"), Category("Cinema")]
public partial class CinemaZone : BaseTrigger
{
    /// <summary>
    /// The projector entity that will play in this area
    /// </summary>
    [Property]
    public EntityTarget Projector { get; set; }

    public ProjectorEntity ProjectorEntity => Projector.GetTarget<ProjectorEntity>();
}
