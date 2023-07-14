﻿using Sandbox;
using System;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;
public abstract partial class WebSurfaceVideoPlayer : IVideoPlayer
{
    [ConVar.Client("projector.websurface.size")]
    public static Vector2 WebSurfaceSize { get; set; } = new Vector2(1280, 720);

    public virtual Texture Texture { get; protected set; }

    public abstract bool IsPaused { get; }
    protected virtual WebSurface WebSurface { get; set; }
    protected virtual bool WebSurfaceMouseClickedDown { get; set; }
    protected MediaRequest RequestData { get; set; }
    protected bool VideoLoaded { get; set; }
    public WebSurfaceVideoPlayer(MediaRequest requestData)
    {
        RequestData = requestData;
    }

    public virtual async Task InitializePlayer()
    {
        WebSurface = Game.CreateWebSurface();
        WebSurface.Size = WebSurfaceSize;
        WebSurface.OnTexture += UpdateTexture;
        WebSurface.InBackgroundMode = false;
        WebSurface.Url = RequestData["Url"];
        await WaitUntilReady();
    }

    protected virtual async Task WaitUntilReady()
    {
        while(!VideoLoaded)
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

    public virtual SoundHandle PlayAudio(IEntity entity)
    {
        // WebSurface will never play positional audio.
        return default;
    }

    public virtual void SetVolume(float newVolume)
    {
        // This is dependent on the website.
    }

    public virtual void SetPosition(Vector3 newPosition)
    {
        // WebSurface will never play positional audio.
    }

    public virtual void Resume() { }
    public virtual void Seek(float time) { }
    public virtual void SetPaused(bool paused) { }

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
