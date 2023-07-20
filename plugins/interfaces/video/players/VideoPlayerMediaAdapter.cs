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

    public virtual Texture Texture { get; protected set; }
    protected virtual VideoPlayer _VideoPlayer { get; set; }
    protected string VideoPath { get; set; }
    protected SoundHandle? CurrentlyPlayingAudio;
    protected IEntity CurrentSoundSource { get; set; }
    protected float LastVolume { get; set; }
    protected bool IsInitializing { get; set; }
    protected virtual bool VideoHasHitched => !IsPaused && VideoLoaded && VideoLastUpdated > 1f;
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
        if (!VideoLoaded)
            Log.Info($"Video is now loaded: {size.x}x{size.y}");

        if (Texture == null || Texture.Size != size)
        {
            InitializeTexture(size);
        }
        Texture.Update(span, 0, 0, (int)size.x, (int)size.y);
        VideoLoaded = true;
        VideoLastUpdated = 0;
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

    public virtual void Seek(float time)
    {
        // To allow for <c>Seek</c> to be called for as long as a video is not synched,
        // we return early here if the video is not loaded.
        if (!VideoLoaded)
            return;

        // Seeking an online video will likely cause the video to hitch, which
        // causes the default anti-hitch mechanism to seek again, possibly forever.
        // To prevent this, make sure we set a flag seen by <c>VideoHasHitched</c>.
        VideoLoaded = false;
        _VideoPlayer?.Seek(time);
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

    /// <summary>
    /// By default, this is called when <c>VideoHasHitched</c> returns true.
    /// This method will call <c>Refresh</c> to fix the hitch.
    /// </summary>
    protected virtual void FixVideoHitch()
    {
        Log.Info("Video hitch detected. Seeking to current time.");
        VideoLoaded = false;
        Refresh();
    }

    [GameEvent.Client.Frame]
    public virtual void OnFrame()
    {
        _VideoPlayer?.Present();

        // When streaming a video from certain streaming websites, it may hitch. This
        // also occurs in VLC, so it's not our fault. The workaround is to refresh the video.
        if (VideoHasHitched)
        {
            FixVideoHitch();
        }
    }
}
