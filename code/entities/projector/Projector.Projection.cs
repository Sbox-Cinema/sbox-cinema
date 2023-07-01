using System;
using CinemaTeam.Plugins.Video;
using Sandbox;
using Sandbox.UI;

namespace Cinema;

public partial class ProjectorEntity
{
    /// <summary>
    /// The media we want to be playing (but might not be)
    /// </summary>
    public IVideoPresenter CurrentMedia { get; protected set; }
    private SpotLightEntity ProjectionLight { get; set; }
    public Texture ProjectionTexture { get; set; }
    protected Texture LastProjectionTexture { get; set; }
    [Net]
    public Vector3 ScreenPosition { get; set; }
    [Net]
    public float ScreenDistance { get; set; }
    protected bool ShouldRemakeLight { get; set; }

    public void SetMedia(IVideoPresenter media)
    {
        if (CurrentMedia != null && CurrentMedia is IVideoControls controls)
        {
            // TODO: Hack, move this logic in to the media controller.
            controls.Stop();
        }
        // Stop now in case the next media doesn't clobber the current overhead audio.
        CurrentOverheadSound?.Stop(true);
        CurrentMedia = media;
        PlayCurrentMedia();
    }

    private void InitProjection()
    {
        SetupProjectionLight();
        Area?.MediaController?.SetWaitingImage();
    }

    private void SetupProjectionLight()
    {
        var outerConeAngle = 60f;
        var innerConeAngle = 45f;
        if (ProjectionLight.IsValid())
        {
            outerConeAngle = ProjectionLight.OuterConeAngle;
            innerConeAngle = ProjectionLight.InnerConeAngle;
        }
        ProjectionLight?.Delete();

        ProjectionLight = new SpotLightEntity
        {
            Parent = this,
            Position = Position,
            Rotation = Rotation,
            LightCookie = ProjectionTexture,
            Brightness = 20.0f,
            Range = 1024.0f,
            OuterConeAngle = outerConeAngle,
            InnerConeAngle = innerConeAngle,
            DynamicShadows = true,
            FogStrength = 1.0f,
            Transmit = TransmitType.Always,
        };

        ProjectionLight.UseFog();
        ProjectionLight.Components.Create<FakeBounceLight>();
    }

    public bool CanSeeProjector(Vector3 pos)
    {
        // Orphaned projectors can't play media, so return null.
        if (Area == null)
            return false;

        var inside = Area.WorldSpaceBounds.Contains(pos);
        return inside;
    }

    [GameEvent.Tick.Server]
    protected void OnServerTick()
    {
        UpdateScreenPositionAndDistance();
    }

    [GameEvent.Tick.Client]
    protected virtual void UpdateClientProjection()
    {
        if (ProjectionLight == null)
            return;

        UpdateProjectorAngles();

        // If the projection texture changed, remake the projector and bounce lights.
        if (ProjectionTexture != LastProjectionTexture)
        {
            Log.Info($"{Name} - Projection texture changed.");
            InitProjection();
        }
        LastProjectionTexture = ProjectionTexture;
    }

    protected void UpdateScreenPositionAndDistance()
    {
        const float maxDistance = 5000f;

        // Trace forward from the projector light to find the surface it projects on to.
        var traceStart = Position;
        var traceEnd = Position + Rotation.Forward * maxDistance;
        var tr = Trace.Ray(traceStart, traceEnd)
            .StaticOnly()
            .Run();

        // If nothing hit, we act as if the screen is somewhere far in front of the projector.
        ScreenPosition = tr.Hit
            ? tr.HitPosition
            : traceEnd;

        ScreenDistance = tr.Hit
            ? tr.Distance
            : maxDistance;
    }

    protected void UpdateProjectorAngles()
    {
        var largestSideSize = MathF.Max(ProjectionSize.x, ProjectionSize.y);
        var smallestSideSize = MathF.Min(ProjectionSize.x, ProjectionSize.y);
        var aspectRatio = largestSideSize / smallestSideSize;
        // Scale up the spotlight to try to fit the all corners of the screen,
        // then half it for math reasons.
        var spotLightRadius = largestSideSize * aspectRatio / 2f;
        // Pretty much gets the visual angle/angular diameter of the screen as seen by the projector,
        // assuming you look at it head-on. Good enough approximation for now.
        var angle = MathF.Atan(spotLightRadius / ScreenDistance);
        var outerConeAngle = MathX.RadianToDegree(angle);

        ProjectionLight.OuterConeAngle = outerConeAngle;
        // Just make the inner cone angle smaller.
        ProjectionLight.InnerConeAngle = ProjectionLight.OuterConeAngle * 0.5f;
    }

    private void PlayCurrentMedia()
    {
        if (!Game.LocalPawn.IsValid()) return;

        if (!CanSeeProjector(Game.LocalPawn.Position))
        {
            Log.Info($"{Name} - Cleaning up projection.");
            CleanupProjection();
            return;
        }

        if (CurrentMedia?.Texture == null)
        {
            Log.Info($"{Name} - Media texture is null.");
            return;
        }

        ProjectionTexture = CurrentMedia.Texture;
        SetupProjectionLight();
    }

    protected void CleanupProjection()
    {
        ProjectionTexture?.Dispose();
    }
}
