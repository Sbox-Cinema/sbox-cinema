namespace CinemaTeam.Plugins.Video;

/// <summary>
/// 
/// </summary>
public interface IVideoControls
{
    void Resume();
    void SetPaused(bool paused);
    void Stop();
    void Seek(float time);
}
