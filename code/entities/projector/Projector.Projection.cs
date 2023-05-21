using System;
using Sandbox;
using Sandbox.UI;

namespace Cinema;

public partial class ProjectorEntity
{
    private WebSurface WebSurface;

    public string WebSurfaceUrl => WebSurface?.Url;
    public string WebSurfaceVideoId { get; protected set; }

    public string CurrentVideoId
    {
        get => _CurrentVideoId;
        protected set
        {
            _CurrentVideoId = value;
            _CurrentStaticUrl = null;
        }
    }
    private string _CurrentVideoId;
    public string CurrentStaticUrl
    {
        get => _CurrentStaticUrl;
        protected set
        {
            _CurrentStaticUrl = value;
            _CurrentVideoId = null;
        }
    }
    private string _CurrentStaticUrl;
    public bool PlayingYouTubeVideo => CurrentVideoId != null;
    public bool ShowingStaticImage => CurrentStaticUrl != null;

    private Texture WebSurfaceTexture { get; set; }
    private OrthoLightEntity ProjectionLight { get; set; }
    private Texture ProjectionTexture { get; set; }

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

        SetWebSurfaceUrl();

        ProjectionTexture = Texture.CreateRenderTarget("projection", ImageFormat.RGBA8888, ProjectionResolution);

        SetupProjectionLight();
    }

    private void SetWebSurfaceUrl(string url = null)
    {
        if (WebSurface != null)
        {
            WebSurface.Url = url;
            return;
        };

        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = ProjectionResolution;
        WebSurface.InBackgroundMode = false;
        WebSurface.OnTexture = UpdateWebTexture;
        WebSurface.Url = url ?? "https://i.pinimg.com/originals/62/c7/c2/62c7c28439ff95418a16b0d0c907fa18.jpg";
    }

    public void SetStaticImage(string url)
    {
        if (CurrentStaticUrl == url) return;

        CurrentStaticUrl = url;

        PlayContentOnProjector();
    }

    public void PlayYouTubeVideo(string id)
    {
        if (CurrentVideoId == id) return;

        CurrentVideoId = id;

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

    public void PlayContentOnProjector()
    {
        if (!CanSeeProjector(Game.LocalPawn.Position))
        {
            WebSurface?.Dispose();
            WebSurface = null;
            WebSurfaceVideoId = null;
            return;
        }

        if (PlayingYouTubeVideo && WebSurfaceVideoId != CurrentVideoId)
        {
            SetWebSurfaceUrl($"https://cinema-api.fly.dev/player.html?dt={CurrentVideoId}&vol=100");
            WebSurfaceVideoId = CurrentVideoId;
            return;
        }

        if (ShowingStaticImage && WebSurface?.Url != CurrentStaticUrl)
        {
            WebSurfaceVideoId = null;
            SetWebSurfaceUrl(CurrentStaticUrl);
            return;
        }
    }

    private bool clickedDown = false;

    [GameEvent.Tick.Client]
    protected void TickClient()
    {
        clickedDown = !clickedDown;
        if (WebSurface != null)
        {
            WebSurface.TellMouseButton(MouseButtons.Left, clickedDown);
        }
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
