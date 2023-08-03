﻿using System.Collections.Generic;
using System.Linq;
using CinemaTeam.Plugins.Media;
using Editor;
using Sandbox;
using Sandbox.UI;

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
    public ProjectorOverlayPanel OverlayPanel { get; set; }

    [Net]
    public CinemaZone Zone { get; set; }
    protected Vector3 OverheadAudioPosition => Position + Rotation.Forward * (ScreenDistance / 2);

    public override void Spawn()
    {
        base.Spawn();

        // Always transmit to clients, as we'll manually control the creation/destruction of
        // resources when a player enters or exits a CinemaZone.
        Transmit = TransmitType.Always;

        if (ProjectionSize == default)
        {
            ProjectionSize = new Vector2(320, 180);
        }
        // Force a 16:9 aspect ratio.
        if (ProjectionSize.x % 16 != 0 || ProjectionSize.y % 9 != 0)
        {
            var newSize = Vector2.Zero;
            newSize.x = ProjectionSize.x - (ProjectionSize.x % 16);
            newSize.y = newSize.x * (9.0f / 16.0f);
            ProjectionSize = newSize;
        }
    }

    [GameEvent.Entity.PostSpawn]
    protected void PostSpawn()
    {
        Zone = All.OfType<CinemaZone>().FirstOrDefault(area => area.ProjectorEntity == this);
        if (Zone == null)
        {
            Log.Info($"Map error: projector {Name} does not belong to a CinemaZone.");
        }
    }

    [GameEvent.Tick.Client]
    public void OnClientTick()
    {
        UpdateClientProjection();
        if (OverlayPanel.IsValid())
        {
            OverlayPanel.Position = ScreenPosition + ScreenNormal;
            OverlayPanel.Rotation = Rotation.LookAt(ScreenNormal, ScreenNormal);
        }
    }

    [ClientRpc]
    public void ClientInitialize()
    {
        InitializeProjection();
        InitializeOverlay();
        ProjectCurrentMedia();
    }

    private void InitializeOverlay()
    {
        OverlayPanel = new ProjectorOverlayPanel();
        var resolutionScale = 0.15f;
        var scaledProjectionSize = ProjectionSize / ScenePanelObject.ScreenToWorldScale * resolutionScale;
        OverlayPanel.PanelBounds = new Rect(Vector2.Zero - scaledProjectionSize / 2, scaledProjectionSize);
        OverlayPanel.WorldScale = 1 / resolutionScale;
    }

    private void CleanupOverlay()
    {
        OverlayPanel.Delete();
    }

    [ClientRpc]
    public void ClientCleanup()
    {
        CleanupProjection();
        CleanupOverlay();
    }

    public void SetMedia(IMediaPlayer media)
    {
        // Stop whatever media might already be playing.
        CurrentMedia?.Stop();
        CurrentMedia = media;
        OverlayPanel.Media = media;
        ProjectCurrentMedia();
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

