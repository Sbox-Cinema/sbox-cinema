using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_projector"), HammerEntity]
[EditorModel("models/editor/ortho", "rgb(0, 255, 192)", "rgb(255, 64, 64)")]
[Title("Projector"), Category("Gameplay"), Icon("monitor")]
[SupportsSolid]
public partial class ProjectorEntity : Entity
{
    [Property(Title = "Projector Name")]
    public string ProjectorName { get; set; } = "Projector";

    [Property(Title = "Projection Resolution (Pixels)")]
    public Vector2 ProjectionResolution { get; set; } = new Vector2(1024, 1024);

    [Property(Title = "Projection Size (Units)")]
    public Vector2 ProjectionSize { get; set; } = new Vector2(480, 256);

    public MediaController Controller { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        Rotation = Rotation.FromYaw(90);

        Controller = new MediaController()
        {
            Projector = this
        };
        Controller.Parent = this;
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();
        InitProjection();
    }
}

