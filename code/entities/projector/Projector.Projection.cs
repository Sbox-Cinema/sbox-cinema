using System;
using Sandbox;

namespace Cinema;

public partial class ProjectorEntity
{
    private WebSurface WebSurface;

    public string CurrentUrl => WebSurface.Url;

    public string CurrentVideoId { get; protected set; }

    private Texture ProjectionTexture { get; set; }
    private OrthoLightEntity ProjectionLight { get; set; }

    private void InitProjection()
    {
        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = ProjectionResolution;
        WebSurface.InBackgroundMode = false;
        WebSurface.OnTexture = UpdateWebTexture;

        WebSurface.Url = "https://www.youtube.com/embed/XkfmrXLxaNk?autoplay=0;frameborder=0";

        //Initialize Texture
        ProjectionTexture = Texture.Create((int)ProjectionResolution.x, (int)ProjectionResolution.y, ImageFormat.BGRA8888)
                                        .WithName("projection-img")
                                        .WithUAVBinding()
                                        .Finish();

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
        Log.Info("tell");
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
        if (ProjectionTexture == null || ProjectionTexture.Size != size)
        {
            Log.Info("Updating projection texture");
            ProjectionTexture?.Dispose();
            ProjectionTexture = Texture.Create((int)size.x, (int)size.y, ImageFormat.BGRA8888)
                                        .WithName("projection-img")
                                        .WithDynamicUsage()
                                        .WithUAVBinding()
                                        .Finish();
            SetupProjectionLight();
        }
        ProjectionTexture.Update(span, 0, 0, (int)size.x, (int)size.y);
    }
}
