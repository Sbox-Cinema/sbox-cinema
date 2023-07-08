using Sandbox;
using System;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public class DirectVideoPlayer : IVideoPlayer
{
    public DirectVideoPlayer()
    {
        Event.Register(this);
    }

    public virtual Texture Texture { get; protected set; }
    public virtual bool IsPaused => VideoPlayer?.IsPaused ?? false;

    protected virtual VideoPlayer VideoPlayer { get; set; }
    protected string VideoPath { get; set; }
    protected SoundHandle? CurrentlyPlayingAudio;
    protected IEntity CurrentSoundSource { get; set; }
    protected bool IsInitializing { get; set; }
    protected bool VideoLoaded { get; set; }
    protected bool AudioLoaded { get; set; }
    protected TimeSince VideoLastUpdated { get; set; }


    public virtual async Task InitializePlayer(MediaRequest requestData)
    {
        VideoPath = requestData["Url"];
        IsInitializing = true;
        Stop();
        Play(VideoPath);
        await WaitUntilReady();
    }

    protected virtual async Task WaitUntilReady()
    {
        if (!IsInitializing)
            return;

        while (!(VideoLoaded && AudioLoaded))
        {
            await GameTask.DelaySeconds(Time.Delta);
        }
        IsInitializing = false;
    }

    protected virtual void Play(string url)
    {
        VideoPlayer = new VideoPlayer();
        VideoPlayer.OnAudioReady += () =>
        {
            Log.Trace("Audio loaded.");
            AudioLoaded = true;
        };
        VideoPlayer.OnTextureData += OnTextureData;
        VideoPlayer.Play(url);
    }

    protected virtual void OnTextureData(ReadOnlySpan<byte> span, Vector2 size)
    {
        VideoLoaded = true;
        VideoLastUpdated = 0;
        if (Texture == null || Texture.Size != size)
        {
            InitializeTexture(size);
        }

        Texture.Update(span, 0, 0, (int)size.x, (int)size.y);
    }

    protected virtual void InitializeTexture(Vector2 size)
    {
        Texture?.Dispose();
        Texture = Texture.Create((int)size.x, (int)size.y, ImageFormat.RGBA8888)
                                    .WithName("direct-video-player-texture")
                                    .WithDynamicUsage()
                                    .WithUAVBinding()
                                    .Finish();
    }

    public virtual SoundHandle PlayAudio(IEntity entity)
    {
        if (VideoPlayer == null)
            return default;

        CurrentlyPlayingAudio?.Stop(true);
        CurrentSoundSource = entity;
        CurrentlyPlayingAudio = VideoPlayer.PlayAudio(CurrentSoundSource);
        if (CurrentlyPlayingAudio.HasValue)
        {
            var hSnd = CurrentlyPlayingAudio.Value;
            hSnd.Volume = MediaConfig.DefaultMediaVolume;
        }
        return CurrentlyPlayingAudio ?? default;
    }

    public void SetVolume(float newVolume)
    {
        if (CurrentlyPlayingAudio.HasValue)
        {
            var hSnd = CurrentlyPlayingAudio.Value;
            hSnd.Volume = newVolume;
        }
    }

    public virtual void Resume()
    {
        VideoPlayer?.Resume();
    }

    public virtual void SetPaused(bool paused)
    {
        if (VideoPlayer == null)
            return;

        if (VideoPlayer.IsPaused != paused)
        {
            VideoPlayer.TogglePause();
        }
    }

    public virtual void Seek(float time)
    {
        VideoPlayer?.Seek(time);
    }

    public virtual void Stop()
    {
        if (VideoPlayer == null)
            return;

        CurrentlyPlayingAudio?.Stop(true);
        AudioLoaded = false;
        VideoPlayer.Stop();
        VideoPlayer.Dispose();
        VideoLoaded = false;
        VideoPlayer = null;
    }

    protected virtual void Refresh()
    {
        IsInitializing = true;
        var currentTime = VideoPlayer.PlaybackTime;
        Stop();
        Play(VideoPath);
        GameTask.RunInThreadAsync(async () =>
        {
            await WaitUntilReady();
            Seek(currentTime);
            PlayAudio(CurrentSoundSource);
        });
    }

    [GameEvent.Client.Frame]
    public virtual void OnFrame()
    {
        VideoPlayer?.Present();
    }
}
