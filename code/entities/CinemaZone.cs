using Editor;
using Sandbox;
using System.Collections.Generic;

namespace Cinema;

[Library("cinema_zone"), HammerEntity, Solid]
[Title("Cinema Zone"), Category("Cinema")]
public partial class CinemaZone : BaseTrigger
{
    /// <summary>
    /// The projector entity that will play in this area
    /// </summary>
    [Property]
    public EntityTarget Projector { get; set; }
    [BindComponent]
    public MediaController MediaController { get; }
    [Net]
    public ProjectorEntity ProjectorEntity { get; set; }

    /// <summary>
    /// Returns true if this zone has a projector and media controller.
    /// </summary>
    public bool IsTheaterZone => MediaController != null && ProjectorEntity != null;

    public override void Spawn()
    {
        base.Spawn();
        Transmit = TransmitType.Always;
    }

    [GameEvent.Entity.PostSpawn]
    public void OnPostSpawn()
    {
        ProjectorEntity = Projector.GetTarget<ProjectorEntity>();
        Components.Create<MediaController>();
    }

    public override void OnTouchStart(Entity toucher)
    {
        base.OnTouchStart(toucher);

        if (toucher is Player ply)
        {
            ply.EnterZone(this);
        }
    }

    public override void OnTouchEnd(Entity toucher)
    {
        base.OnTouchEnd(toucher);

        if (toucher is Player ply)
        {
            ply.ExitZone(this);
        }
    }
}
