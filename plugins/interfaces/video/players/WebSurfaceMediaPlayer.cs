using Sandbox;
using System;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;
public partial class WebSurfaceMediaPlayer : IMediaPlayer, IVideoPlayer
{
    [ConVar.Client("projector.websurface.size")]
    public static Vector2 WebSurfaceSize { get; set; } = new Vector2(1280, 720);

    public virtual IVideoPlayer VideoPlayer => this;
    public virtual Texture Texture { get; protected set; }

    // Volume depends on the website, and audio position may never be set.
    public virtual IAudioPlayer AudioPlayer => null; 
    // The playback controls would be implemented per-website.
    public virtual IPlaybackControls Controls => null;
    public virtual bool VideoLoaded { get; protected set; }

    protected WebSurface WebSurface { get; set; }
    protected bool WebSurfaceMouseClickedDown { get; set; }
    protected MediaRequest RequestData { get; set; }

    public WebSurfaceMediaPlayer()
    {
        Event.Register(this);
    }

    public virtual async Task StartAsync(MediaRequest request)
    {
        RequestData = request;
        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = WebSurfaceSize;
        WebSurface.OnTexture += UpdateTexture;
        WebSurface.InBackgroundMode = false;
        WebSurface.Url = RequestData["Url"];
        await WaitUntilReady();
    }

    protected virtual async Task WaitUntilReady()
    {
        while (!VideoLoaded)
        {
            await GameTask.DelaySeconds(Time.Delta);
        }
    }

    protected virtual void UpdateTexture(ReadOnlySpan<byte> span, Vector2 size)
    {
        if (Texture == null || Texture.Size != size)
        {
            Log.Trace($"Creating texture with size {size} and length {span.Length}");
            VideoLoaded = true;
            InitializeTexture(size);
        }

        Texture.Update(span, 0, 0, (int)size.x, (int)size.y);
    }

    protected virtual void InitializeTexture(Vector2 size)
    {
        Texture?.Dispose();
        Texture = Texture.Create((int)size.x, (int)size.y, ImageFormat.BGRA8888)
                                    .WithName("web-surface-texture")
                                    .WithDynamicUsage()
                                    .WithUAVBinding() // Needed for the texture to work as a light cookie.
                                    .Finish();
    }

    public virtual void Stop()
    {
        if (WebSurface == null)
            return;

        WebSurface?.Dispose();
        WebSurface = null;
    }

    [GameEvent.Tick.Client]
    public virtual void OnClientTick()
    {
        WebSurfaceMouseClickedDown = !WebSurfaceMouseClickedDown;
        WebSurface?.TellMouseButton(MouseButtons.Left, WebSurfaceMouseClickedDown);
    }
}
