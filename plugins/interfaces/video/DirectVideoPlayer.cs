using Sandbox;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;
public class DirectVideoPlayer : IVideoPlayer
{
    public DirectVideoPlayer(MediaRequest requestData)
    {
        Event.Register(this);
    }
    
    protected bool VideoLoaded { get; set; }
    protected bool AudioLoaded { get; set; }

    public virtual async Task InitializePlayer(MediaRequest requestData)
    {
        VideoPlayer = new VideoPlayer();
        var url = requestData["Url"];
        VideoPlayer.OnLoaded += async () =>
        {
            Log.Trace("Video loaded.");
            // It's possible that after the video is loaded, the texture is still 1x1 pixels.
            if (Texture.Width == 1 && Texture.Height == 1)
            {
                const int maxTries = 5;
                // We just spin for a few frames to see if the texture gets bigger.
                for (int i = 0; i < maxTries; i++)
                {
                    Log.Trace($"Video is {Texture.Width}x{Texture.Height} now.");
                    await GameTask.DelaySeconds(Time.Delta);
                    if (Texture.Width > 1 && Texture.Height > 1)
                        break;
                }
            }
            Log.Trace("Video loaded.");
            VideoLoaded = true;
        };
        VideoPlayer.OnAudioReady += () =>
        {
            Log.Trace("Audio loaded.");
            AudioLoaded = true;
        };
        PlayUrl(url);
        while (!(VideoLoaded && AudioLoaded))
        {
            await GameTask.DelaySeconds(Time.Delta);
        }
    }

    protected virtual void PlayUrl(string url)
    {
        VideoPlayer.Play(url);
    }

    protected virtual VideoPlayer VideoPlayer { get; set; }

    public virtual Texture Texture => VideoPlayer?.Texture;
    public virtual SoundHandle? PlayAudio(IEntity entity)
    {
        return VideoPlayer?.PlayAudio(entity);
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

        VideoPlayer.Stop();
        VideoPlayer.Dispose();
        VideoPlayer = null;
    }

    [GameEvent.Client.Frame]
    public void OnFrame()
    {
        VideoPlayer?.Present();
    }
}
