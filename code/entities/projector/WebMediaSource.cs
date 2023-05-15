using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema;

public partial class WebMediaSource : WorldPanel
{
    private WebSurface WebSurface;

    public string CurrentUrl
    {
        get { return WebSurface.Url; }
        set { WebSurface.Url = value; }
    }

    private Texture WebTexture;

    public WebMediaSource()
    {
        InitWebSurface();
    }

    private void InitWebSurface()
    {
        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = new Vector2(1600, 900);
        WebSurface.InBackgroundMode = false;
        WebSurface.OnTexture = OnBrowserDataChanged;

        WebSurface.Url = "https://www.youtube.com/embed/XkfmrXLxaNk?autoplay=0;frameborder=0";
    }

    void OnBrowserDataChanged(ReadOnlySpan<byte> span, Vector2 size)
    {
        if (Game.IsServer)
        {
            return;
        }

        UpdateWebTexture(span, size);
    }

    private void UpdateWebTexture(ReadOnlySpan<byte> span, Vector2 size)
    {
        if (WebTexture == null || WebTexture.Size != size)
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

    public override void OnDeleted()
    {
        base.OnDeleted();

        WebTexture?.Dispose();
        WebTexture = null;

        WebSurface?.Dispose();
    }
}
