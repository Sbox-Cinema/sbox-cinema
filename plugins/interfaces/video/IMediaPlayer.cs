using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Provides a media player that can be used to play a <c>MediaRequest</c>.
/// </summary>
public interface IMediaPlayer
{
    /// <summary>
    /// Starts playing the media specified by the request.
    /// </summary>
    /// <param name="request">The media that shall be played.</param>
    Task StartAsync(MediaRequest request);
    /// <summary>
    /// If not null, provides a texture. This is used for videos, images, and audio visualizers.
    /// </summary>
    IVideoPlayer VideoPlayer { get; }
    /// <summary>
    /// If not null, provides an audio source. This is used for videos and music players.
    /// </summary>
    IAudioPlayer AudioPlayer { get; }
    /// <summary>
    /// If not null, provides playback controls which allow users to pause, resume, 
    /// and arbitrarily seek through media. This will probably be null for radio-style
    /// streams and still images.
    /// </summary>
    IPlaybackControls Controls { get; }
    /// <summary>
    /// Stops playback of the media, disposing any resources that were used. After this 
    /// method is called, the media player should not be used again.
    /// </summary>
    void Stop();
}
