using System.Collections.Generic;
using System.Linq;
using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_projector"), HammerEntity]
[EditorModel("models/editor/ortho", "rgb(0, 255, 192)", "rgb(255, 64, 64)")]
[Title("Projector"), Category("Cinema"), Icon("monitor")]
[SupportsSolid]
public partial class ProjectorEntity : Entity
{
    [Property(Title = "Projector Name")]
    public string ProjectorName { get; set; } = "Projector";

    [Net, Property(Title = "Projection Size (Units)")]
    public Vector2 ProjectionSize { get; set; }

    [Net]
    public IList<CinemaZone> Areas { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        if (ProjectionSize == default)
        {
            ProjectionSize = new Vector2(256, 208);
        }

        Components.Create<MediaController>();
    }

    [GameEvent.Entity.PostSpawn]
    protected void PostSpawn()
    {
        Areas = All.OfType<CinemaZone>().Where(area => area.ProjectorEntity == this).ToList();
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();
        InitProjection();
    }

    protected override void OnDestroy()
    {
        CleanupProjection();
    }

    public static void DrawGizmos(EditorContext context)
    {
        if (!context.IsSelected)
        {
            return;
        }

        var projectionSizeProp = context.Target.GetProperty("ProjectionSize");
        var projectionSize = projectionSizeProp.GetValue<Vector2>();

        var length = 3000f;
        var mins = new Vector3(0, -(projectionSize.x / 2), -(projectionSize.y / 2));
        var maxs = new Vector3(length, projectionSize.x / 2, projectionSize.y / 2);
        var bbox = new BBox(mins, maxs);
        Gizmo.Draw.LineBBox(bbox);
    }
}

