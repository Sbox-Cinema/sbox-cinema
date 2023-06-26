using Sandbox;

namespace CinemaTeam.Plugins.Video;
public interface IVideoPresenter
{
    Texture Texture { get; }
    SoundHandle? PlayAudio(IEntity entity);
}
