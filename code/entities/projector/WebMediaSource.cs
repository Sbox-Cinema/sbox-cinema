using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema;

public partial class WebMediaSource : WorldPanel
{
    private ProjectorEntity Projector;

    private WebSurface WebSurface;
    public Texture WebTexture;

    public string CurrentUrl => WebSurface.Url;

    public WebMediaSource(ProjectorEntity ent)
    {
        Projector = ent;

        InitWorldPanel();
        InitWebSurface();
    }
    public WebMediaSource(ProjectorEntity ent, SceneWorld world = null) : base(world)
    {
        Projector = ent;

        InitWorldPanel();
        InitWebSurface();
    }
    private void InitWorldPanel()
    {
        PanelBounds = new Rect(-Projector.ProjectionResolution.x / 2, -Projector.ProjectionResolution.y / 2, Projector.ProjectionResolution.x, Projector.ProjectionResolution.y);
    }
    private void InitWebSurface()
    {
        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = Projector.ProjectionResolution;
        WebSurface.InBackgroundMode = false;
        WebSurface.OnTexture = OnBrowserDataChanged;

        WebSurface.Url = "https://www.youtube.com/embed/XkfmrXLxaNk?autoplay=0;frameborder=0";
    }
    void ResetWebPanel()
    {
        WebSurface.Url = "https://www.youtube.com/embed/XkfmrXLxaNk?autoplay=0;frameborder=0";
    }

    public void SetUrl(string url)
    {
        WebSurface.Url = url;
    }

    public override void OnHotloaded()
    {
        ResetWebPanel();
    }
    private void UpdateWebTexture(ReadOnlySpan<byte> span, Vector2 size)
    {
        if ( WebTexture == null || WebTexture.Size != size )
        {
            WebTexture?.Dispose();
            WebTexture = null;
            WebTexture = Texture.Create((int)size.x, (int)size.y, ImageFormat.BGRA8888)
                .WithDynamicUsage()
                .WithName("Web")
                .Finish();

            Style.SetBackgroundImage(WebTexture);
        }
        WebTexture.Update(span, 0, 0, (int)size.x, (int)size.y);
    }
    void OnBrowserDataChanged(ReadOnlySpan<byte> span, Vector2 size)
    {
        if ( Game.IsServer )
        {
            return;
        }

        UpdateWebTexture(span, size);
    }
    public override void OnDeleted()
    {
        base.OnDeleted();

        WebTexture?.Dispose();
        WebTexture = null;

        WebSurface?.Dispose();
    }
}
