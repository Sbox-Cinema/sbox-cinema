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
}
