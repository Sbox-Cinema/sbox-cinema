namespace CinemaTeam.Plugins.Video;

/// <summary>
/// Provides a union of the IVideoPresenter and IMediaControls interfaces. 
/// TODO: See whether this can be removed.
/// </summary>
public interface IVideoPlayer : IVideoPresenter, IMediaControls { }
