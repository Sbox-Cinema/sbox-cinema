using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

/// <summary>
/// Provides a panel that can be used to generate queries for a media provider.
/// </summary>
public interface IMediaSelector
{
    public class MediaRequestEventArgs : EventArgs
    {
        public string Query { get; init; }
    }

    /// <summary>
    /// The panel to display when this provider is selected in the media queue menu.
    /// </summary>
    MediaProviderHeaderPanel HeaderPanel { get; }
}
