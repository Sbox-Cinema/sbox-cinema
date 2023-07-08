using Sandbox;
using Sandbox.Internal;
using System;

namespace CinemaTeam.Plugins.Video;

public class DummyVideoPresenter : IVideoPlayer
{
    [ConVar.Client("plugins.video.test.drawtex")]
    public static bool EnableShaderDebug { get; set; }
    public Texture MultiplicandTexture { get; set; }
    public Texture Texture { get; private set; }
    public bool IsPaused { get; private set; }
    private SoundHandle CurrentlyPlayingSound { get; set; }
    private IEntity LastEntity { get; set; } = null;

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
        CurrentlyPlayingSound.Stop(true);
        LastEntity = entity;
        CurrentlyPlayingSound = Audio.Play("waltz_of_the_flowers", entity);
        SetVolume(MediaConfig.DefaultMediaVolume);
        return CurrentlyPlayingSound;
    }

    public void SetVolume(float newVolume)
    {
        var hSnd = CurrentlyPlayingSound;
        hSnd.Volume = 5 * newVolume;
    }

    [GameEvent.Client.Frame]
    public void OnFrame()
    {
        if (IsPaused)
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
        PlayAudio(LastEntity);    
    }

    public void SetPaused(bool newPauseValue) 
    {
        var oldPauseValue = IsPaused;
        IsPaused = newPauseValue;

        //if (oldPauseValue == newPauseValue)
        //    return;

        if (newPauseValue)
        {
            CurrentlyPlayingSound.Stop(true);
        }
        else
        {
            Resume();
        }
    }
    public void Stop() 
    {
        CurrentlyPlayingSound.Stop(true);
    }
    public void Seek(float time) { }
}
