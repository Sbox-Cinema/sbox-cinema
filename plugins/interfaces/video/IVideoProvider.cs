using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public interface IVideoProvider
{
    string ProviderName { get; }

    // TODO: Add a thumbnail texture.

    Task<IVideoPlayer> Play(MediaRequest requestData);
}
