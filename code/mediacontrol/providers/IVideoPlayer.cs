namespace CinemaTeam.Plugins.Video;

public interface IVideoPlayer : IVideoPresenter
{
    void Resume();
    void SetPaused(bool paused);
    void Stop();
    void Seek(float time);
}
