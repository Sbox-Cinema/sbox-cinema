using Sandbox;

namespace Cinema;
public interface IVideoPresenter
{
    Texture Texture { get; }
    SoundHandle? PlayAudio(IEntity entity);
}
