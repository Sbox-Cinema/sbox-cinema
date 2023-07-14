namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Allows for the playback position of a video to be controlled,
/// either by seeking to a different time or pausing/resuming.
/// </summary>
public interface IPlaybackControls
{
    float PlaybackTime { get; }
    bool IsPaused { get; }
    void Pause();
    void Resume();
    void Seek(float time);
}
