using Sandbox;
using System;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

public class VideoPlayerMediaAdapter : IMediaPlayer, IVideoPlayer, IAudioPlayer, IPlaybackControls
{
    public VideoPlayerMediaAdapter()
    {
        Event.Register(this);
    }

    public virtual IVideoPlayer VideoPlayer => this;
    public virtual IAudioPlayer AudioPlayer => this;

    public virtual IPlaybackControls Controls => this;
    public virtual void Pause() => _VideoPlayer.Pause();
    public virtual void Resume() => _VideoPlayer?.Resume();
    public virtual bool IsPaused => _VideoPlayer?.IsPaused ?? false;
    public float PlaybackTime
    {
        get => _VideoPlayer?.PlaybackTime ?? 0;
        set => Seek(value);
    }
    public virtual void Seek(float time) => _VideoPlayer?.Seek(time);

    public virtual Texture Texture { get; protected set; }
    protected virtual VideoPlayer _VideoPlayer { get; set; }
    protected string VideoPath { get; set; }
    protected SoundHandle? CurrentlyPlayingAudio;
    protected IEntity CurrentSoundSource { get; set; }
    protected float LastVolume { get; set; }
    protected bool IsInitializing { get; set; }
    protected bool VideoLoaded { get; set; }
    protected bool AudioLoaded { get; set; }
    protected TimeSince VideoLastUpdated { get; set; }

    public virtual async Task StartAsync(MediaRequest requestData)
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
        _VideoPlayer = new VideoPlayer();
        _VideoPlayer.OnAudioReady += () =>
        {
            Log.Trace("Audio loaded.");
            AudioLoaded = true;
        };
        _VideoPlayer.OnTextureData += OnTextureData;
        _VideoPlayer.Play(url);
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

    public virtual void PlayAudio(IEntity entity)
    {
        if (_VideoPlayer == null)
            return;

        CurrentlyPlayingAudio?.Stop(true);
        CurrentSoundSource = entity;
        CurrentlyPlayingAudio = _VideoPlayer.PlayAudio(CurrentSoundSource);
        if (CurrentlyPlayingAudio.HasValue)
        {
            var hSnd = CurrentlyPlayingAudio.Value;
            hSnd.Volume = MediaConfig.DefaultMediaVolume;
            LastVolume = MediaConfig.DefaultMediaVolume;
        }
        return;
    }

    public void SetVolume(float newVolume)
    {
        if (CurrentlyPlayingAudio.HasValue)
        {
            var hSnd = CurrentlyPlayingAudio.Value;
            hSnd.Volume = newVolume;
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (CurrentlyPlayingAudio.HasValue)
        {
            var hSnd = CurrentlyPlayingAudio.Value;
            hSnd.Position = position;
        }
    }

    public virtual void Stop()
    {
        if (_VideoPlayer == null)
            return;

        CurrentlyPlayingAudio?.Stop(true);
        AudioLoaded = false;
        _VideoPlayer.Stop();
        _VideoPlayer.Dispose();
        VideoLoaded = false;
        _VideoPlayer = null;
    }

    protected virtual void Refresh()
    {
        IsInitializing = true;
        var currentTime = _VideoPlayer.PlaybackTime;
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
        _VideoPlayer?.Present();
    }
}
