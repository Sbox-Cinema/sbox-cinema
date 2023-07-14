using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

public interface IMediaCurator
{
    Task<MediaRequest> SuggestMedia();
}
