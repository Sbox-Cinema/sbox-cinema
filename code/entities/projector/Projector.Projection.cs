using System;
using Sandbox;
using Sandbox.UI;

namespace Cinema;

public partial class ProjectorEntity
{
    private WebSurface WebSurface;

    public string CurrentUrl => WebSurface.Url;

    public string CurrentVideoId { get; protected set; }

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

        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = ProjectionResolution;
        WebSurface.InBackgroundMode = false;
        WebSurface.OnTexture = UpdateWebTexture;
        WebSurface.Url = "https://i.pinimg.com/originals/62/c7/c2/62c7c28439ff95418a16b0d0c907fa18.jpg";

        ProjectionTexture = Texture.CreateRenderTarget("projection", ImageFormat.RGBA8888, ProjectionResolution);

        SetupProjectionLight();
    }

    public void SetStaticImage(string url)
    {
        if (CurrentUrl == url) return;

        WebSurface.Url = url;
        CurrentVideoId = null;
    }

    public void PlayYouTubeVideo(string id)
    {
        if (CurrentVideoId == id) return;

        CurrentVideoId = id;
        WebSurface.Url = $"https://cinema-api.fly.dev/player.html?dt={id}&st=0&vol=100";
        WebSurface.TellMouseMove(Vector2.One);
        WebSurface.TellMouseButton(MouseButtons.Left, true);
        SpamMouseClicks();
    }

    private async void SpamMouseClicks()
    {
        WebSurface.TellMouseMove(Vector2.One * 10);
        WebSurface.TellMouseButton(MouseButtons.Left, true);
        await GameTask.Delay(10);
        WebSurface.TellMouseMove(Vector2.One * 30);
        WebSurface.TellMouseButton(MouseButtons.Left, false);
        await GameTask.Delay(100);
        WebSurface.TellMouseButton(MouseButtons.Left, true);
        await GameTask.Delay(100);
        WebSurface.TellMouseButton(MouseButtons.Left, false);
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
            Log.Info("Updating projection texture");
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
