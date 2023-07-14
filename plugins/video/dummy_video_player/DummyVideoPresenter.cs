using Sandbox;

namespace CinemaTeam.Plugins.Video;

public class DummyVideoPresenter : IVideoPlayer
{
    [ConVar.Client("plugins.video.test.drawtex")]
    public static bool EnableShaderDebug { get; set; }
    public Texture MultiplicandTexture { get; set; }
    public Texture Texture { get; private set; }
    public bool IsPaused { get; private set; }
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

    public SoundHandle PlayAudio(IEntity entity)
    {
        Stop();
        Music = MusicPlayer.Play(FileSystem.Mounted, "music/waltz_of_the_flowers.mp3");
        Music.Entity = entity;
        SetVolume(MediaConfig.DefaultMediaVolume);
        return default;
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

    public void SetPaused(bool newPauseValue) 
    {
        Music.Paused = newPauseValue;
        IsPaused = newPauseValue;
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
