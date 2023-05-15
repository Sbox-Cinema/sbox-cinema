using System;
using Sandbox;

namespace Cinema;

public partial class ProjectorEntity
{
    private WebSurface WebSurface;

    public string CurrentUrl
    {
        get => WebSurface.Url;
        set => WebSurface.Url = value;
    }
    private Texture ProjectionTexture { get; set; }
    private OrthoLightEntity ProjectionLight { get; set; }

    private void InitProjection()
    {
        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = ProjectionSize;
        WebSurface.InBackgroundMode = false;
        WebSurface.OnTexture = UpdateWebTexture;

        WebSurface.Url = "https://www.youtube.com/embed/XkfmrXLxaNk?autoplay=0;frameborder=0";

        //Initialize Texture
        ProjectionTexture = Texture.CreateRenderTarget("projection-img", ImageFormat.RGBA8888, ProjectionResolution);

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
            ProjectionTexture?.Dispose();
            ProjectionTexture = null;
            ProjectionTexture = Texture.Create((int)size.x, (int)size.y, ImageFormat.BGRA8888)
                .WithDynamicUsage()
                .WithName("Web")
                .Finish();
        }
        ProjectionTexture.Update(span, 0, 0, (int)size.x, (int)size.y);
    }
}
