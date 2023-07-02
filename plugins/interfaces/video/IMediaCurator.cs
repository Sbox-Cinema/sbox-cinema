using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public interface IMediaCurator
{
    Task<MediaRequest> SuggestMedia();
}
