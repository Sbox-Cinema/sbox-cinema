using System;
using Sandbox;
using Sandbox.UI;

namespace Cinema;

public partial class ProjectorEntity
{
    /// <summary>
    /// The media we want to be playing (but might not be)
    /// </summary>
    public PlayingMedia CurrentMedia { get; protected set; }

    public void SetMedia(PlayingMedia media)
    {
        CurrentMedia = media;
        PlayContentOnProjector();
    }

    public bool CanSeeProjector(Vector3 pos)
    {
        foreach (var area in Areas)
        {
            var inside = area.WorldSpaceBounds.Contains(pos);
            if (inside) return true;
        }

        return false;
    }

    private WebSurface WebSurface;
    /// <summary>
    /// The media on our web surface (if we have one)
    /// </summary>
    private PlayingMedia WebSurfaceMedia { get; set; }
    public Texture WebSurfaceTexture { get; protected set; }
    private SpotLightEntity ProjectionLight { get; set; }
    public Texture ProjectionTexture { get; protected set; }
    private SceneWorld RenderWorld { get; set; }
    private SceneCamera RenderCamera { get; set; }
    private WorldPanel RenderWorldPanel { get; set; }

    private void InitProjection()
    {
        RenderWorld ??= new SceneWorld();
        RenderCamera ??= new SceneCamera()
        {
            World = RenderWorld,
            Position = new Vector3(),
            ZFar = 15000,
            ZNear = 1
        };

        RenderWorldPanel?.Delete();
        RenderWorldPanel = new WorldPanel(RenderWorld)
        {
            Position = RenderCamera.Position + Vector3.Forward * 36f,
            Rotation = Rotation.FromYaw(180f),
            PanelBounds = new Rect(-ProjectionResolution / 2f, ProjectionResolution)
        };

        var waitingImage = new PlayingMedia()
        {
            Url = MediaController.WaitingImage
        };

        SetMedia(waitingImage);

        ProjectionTexture = Texture.CreateRenderTarget("projection", ImageFormat.RGBA8888, ProjectionResolution);

        SetupProjectionLight();
    }

    [GameEvent.Tick.Client]
    public void OnClientTick()
    {
        if (ProjectionLight == null)
            return;

        // Trace forward from the projector light to find the surface it projects on to.
        var traceStart = ProjectionLight.Position;
        var traceEnd = ProjectionLight.Position + ProjectionLight.Rotation.Forward * 5000f;
        var tr = Trace.Ray(traceStart, traceEnd)
            .WorldOnly()
            .Run();
        if (!tr.Hit)
        {
            return;
        }

        var screenDistanceFromProjector = tr.Distance;
        ProjectionLight.OuterConeAngle = CalculateProjectionAngle(screenDistanceFromProjector);
        ProjectionLight.InnerConeAngle = ProjectionLight.OuterConeAngle * 0.8f;
    }

    private float CalculateProjectionAngle(float screenDistanceFromProjector)
    {
        var circleSize = MathF.Min(ProjectionSize.x, ProjectionSize.y);
        var angle = MathF.Atan(circleSize / screenDistanceFromProjector);
        return MathX.RadianToDegree(angle / 2);
    }

    private void PlayMediaOnWebSurface(PlayingMedia media)
    {
        CreateWebSurfaceIfNotExists();

        WebSurfaceMedia = media;
        WebSurface.Url = WebSurfaceMedia.Url;
    }

    private void CreateWebSurfaceIfNotExists()
    {
        if (WebSurface != null) return;
        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = ProjectionResolution;
        WebSurface.InBackgroundMode = false;
        WebSurface.OnTexture = UpdateWebTexture;
    }

    private void PlayContentOnProjector()
    {
        if (!Game.LocalPawn.IsValid()) return;

        if (!CanSeeProjector(Game.LocalPawn.Position))
        {
            WebSurface?.Dispose();
            WebSurface = null;
            WebSurfaceMedia = null;
            return;
        }

        if (WebSurfaceMedia == CurrentMedia)
            return;

        PlayMediaOnWebSurface(CurrentMedia);
    }

    private bool _WebSurfaceMouseClickedDown = false;

    [GameEvent.Tick.Client]
    protected void TickClient()
    {
        _WebSurfaceMouseClickedDown = !_WebSurfaceMouseClickedDown;
        WebSurface?.TellMouseButton(MouseButtons.Left, _WebSurfaceMouseClickedDown);
        PlayContentOnProjector();
    }

    private void SetupProjectionLight()
    {
        ProjectionLight?.Delete();

        ProjectionLight = new SpotLightEntity
        {
            Parent = this,
            Position = Position,
            Rotation = Rotation,
            LightCookie = ProjectionTexture,
            Brightness = 20.0f,
            Range = 1024.0f,
            OuterConeAngle = 20.0f,
            InnerConeAngle = 15.0f,
            DynamicShadows = true,
            FogStrength = 2.0f
        };

        ProjectionLight.UseFog();
        ProjectionLight.Components.Create<FakeBounceLight>();
    }

    private void UpdateWebTexture(ReadOnlySpan<byte> span, Vector2 size)
    {
        if (WebSurfaceTexture == null || WebSurfaceTexture.Size != size)
        {
            WebSurfaceTexture?.Dispose();
            WebSurfaceTexture = Texture.Create((int)size.x, (int)size.y, ImageFormat.BGRA8888)
                                        .WithName("web-surface-texture")
                                        .WithDynamicUsage()
                                        .Finish();
            RenderWorldPanel.Style.SetBackgroundImage(WebSurfaceTexture);
        }

        WebSurfaceTexture.Update(span, 0, 0, (int)size.x, (int)size.y);
    }

    [GameEvent.PreRender]
    protected void OnPreRender()
    {
        Graphics.RenderToTexture(RenderCamera, ProjectionTexture);
    }
}
