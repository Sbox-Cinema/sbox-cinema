using Sandbox;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Identifies a media provider by name and thumbnail.
/// Converts queries in to requests.
/// Provides a video player for a request.
/// </summary>
public interface IMediaProvider
{
    string ProviderName { get; }
    string ThumbnailPath { get; }

    Task<MediaRequest> CreateRequest(IClient client, string queryString);

    Task<IVideoPlayer> Play(MediaRequest requestData);
}
