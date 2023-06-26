using Sandbox;

namespace CinemaTeam.Plugins.Video;
public class DirectVideoPlayer : IVideoPlayer
{
    internal DirectVideoPlayer(string requestData)
    {
        InitializePlayer(requestData);
    }

    protected virtual void InitializePlayer(string requestData)
    {
        VideoPlayer = new VideoPlayer();
        var url = requestData;
        PlayUrl(url);
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
}
