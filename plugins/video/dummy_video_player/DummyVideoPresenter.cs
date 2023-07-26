using Sandbox;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

public class DummyVideoPresenter : IMediaPlayer, IVideoPlayer, IAudioPlayer, IPlaybackControls
{
    [ConVar.Client("plugins.video.test.drawtex")]
    public static bool EnableShaderDebug { get; set; }

    public virtual IVideoPlayer VideoPlayer => this;
    public virtual IAudioPlayer AudioPlayer => this;
    public virtual IPlaybackControls Controls => this;
    public float PlaybackTime 
    {
        get => Music?.PlaybackTime ?? 0;
        set => Seek(value);
    }
    public bool IsPaused => Music?.Paused ?? false;
    public virtual bool VideoLoaded => true;
    public virtual bool AudioLoaded => true;

    public void Pause()
    {
        if (Music == null)
            return;

        Music.Paused = true;
    }

    public Texture MultiplicandTexture { get; set; }
    public Texture Texture { get; private set; }
    private MusicPlayer Music { get; set; }
    private ComputeShader ColorfulShader { get; set; }

    public DummyVideoPresenter()
    {
        ColorfulShader = new ComputeShader("colorfultest_cs");
        MultiplicandTexture = Texture.Load(FileSystem.Mounted, "textures/border_collies.vtex");
        Texture = Texture.Create(MultiplicandTexture.Width, MultiplicandTexture.Height)
            .WithUAVBinding()
            .WithDynamicUsage()
            .WithFormat(ImageFormat.RGBA8888)
            .Finish();
        Event.Register(this);
    }

    public Task StartAsync(MediaRequest request)
    {
        return Task.CompletedTask;
    }

    public void PlayAudio(IEntity entity)
    {
        Stop();
        Music = MusicPlayer.Play(FileSystem.Mounted, "music/waltz_of_the_flowers.mp3");
        Music.Entity = entity;
        SetVolume(MediaConfig.DefaultMediaVolume);
    }

    public void SetVolume(float newVolume)
    {
        if (Music == null)
            return;

        Music.Volume = 5 * newVolume;
    }

    public void SetPosition(Vector3 position)
    {
        if (Music == null)
            return;

        Music.Position = position;
    }

    [GameEvent.Client.Frame]
    public void OnFrame()
    {
        if (Music == null || IsPaused)
            return;

        ColorfulShader.Attributes.Set("GameTime", Time.Now);
        ColorfulShader.Attributes.Set("MultiplicandTexture", MultiplicandTexture);
        ColorfulShader.Attributes.Set("OutputTexture", Texture);
        ColorfulShader.Dispatch(Texture.Width, Texture.Height, 1);
        if (EnableShaderDebug)
        {
            DebugOverlay.Texture(Texture, Vector2.Zero);
        }
    }

    public void Resume() 
    {
        if (Music == null)
            return;

        Music.Paused = false;
    }

    public void Stop()
    {
        if (Music == null)
            return;

        Music.Stop();
        Music.Dispose();
        Music = null;
    }
    public void Seek(float time)
    {
        if (Music == null)
            return;

        Music.Seek(time);
    }
}
