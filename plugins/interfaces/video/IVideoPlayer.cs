using Sandbox;

namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Provides a texture that can be displayed on a screen.
/// </summary>
public interface IVideoPlayer
{
    /// <summary>
    /// A visual representation of this media. If the media is a video, this should be
    /// updated every frame. If the media is a still image, this texture would likely
    /// have been updated only once, during initialization.
    /// </summary>
    Texture Texture { get; }
    /// <summary>
    /// Returns true if the video is fully loaded and updating <c>Texture</c> as normal. 
    /// May return false if the video is buffering or has not yet finished initializing.
    /// </summary>
    bool VideoLoaded { get; }
}
