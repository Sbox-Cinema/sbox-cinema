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
    private OrthoLightEntity ProjectionLight { get; set; }
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

        ProjectionLight = new OrthoLightEntity
        {
            Parent = this,
            Position = Position,
            Rotation = Rotation,
            LightCookie = ProjectionTexture,
            Brightness = 1.0f,
            Range = 1024.0f,
            OrthoLightWidth = ProjectionSize.x,
            OrthoLightHeight = ProjectionSize.y,
            DynamicShadows = true,
        };

        ProjectionLight.UseFog();
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
