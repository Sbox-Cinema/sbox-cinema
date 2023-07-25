using Sandbox;

namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Provides an audio source that can be played.
/// </summary>
public interface IAudioPlayer
{
    void PlayAudio(IEntity entity = null);
    void SetVolume(float newVolume);
    void SetPosition(Vector3 newPosition);
    /// <summary>
    /// Returns true if the audio is fully loaded and playing as normal May return 
    /// false when buffering or during initialization.
    /// </summary>
    bool AudioLoaded { get; }
}
