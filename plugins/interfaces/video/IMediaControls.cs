namespace CinemaTeam.Plugins.Video;

/// <summary>
/// Allows for the playback of a video to be controlled.
/// </summary>
public interface IMediaControls
{
    void Resume();
    void SetPaused(bool paused);
    void Stop();
    void Seek(float time);
}
