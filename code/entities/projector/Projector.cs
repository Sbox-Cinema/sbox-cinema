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

    [ConVar.Client("projector.audio.volumescale")]
    public static float VolumeScale { get; set; } = 3.0f;
    // TODO: Persist VolumeScale clientside.

    [Net]
    public CinemaZone Area { get; set; }
    protected SoundHandle? CurrentOverheadSound { get; set; }
    protected Vector3 OverheadAudioPosition => Position + Rotation.Forward * (ScreenDistance / 2);

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        if (ProjectionSize == default)
        {
            ProjectionSize = new Vector2(256, 208);
        }
    }

    [GameEvent.Entity.PostSpawn]
    protected void PostSpawn()
    {
        Area = All.OfType<CinemaZone>().FirstOrDefault(area => area.ProjectorEntity == this);
        if (Area == null)
        {
            Log.Info($"Map error: projector {Name} does not belong to a CinemaZone.");
        }
    }

    private float OldVolumeScale = VolumeScale;

    [GameEvent.Tick.Client]
    public void OnClientTick()
    {
        if (CurrentOverheadSound != null)
        {
            var hSnd = CurrentOverheadSound.Value;
            hSnd.Position = OverheadAudioPosition;

            if (VolumeScale != OldVolumeScale)
            {
                hSnd.Volume = VolumeScale;
            }
        }
        OldVolumeScale = VolumeScale;
        UpdateClientProjection();
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();
        InitProjection();
    }

    public void PlayOverheadAudio()
    {
        if (CurrentMedia == null)
            return;

        CurrentOverheadSound?.Stop(true);
        var soundPosition = OverheadAudioPosition;
        var hSnd = CurrentMedia.PlayAudio(null).Value;
        hSnd.Position = soundPosition;
        hSnd.Volume = VolumeScale;
        CurrentOverheadSound = hSnd;
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

