using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public interface IMediaSelector
{
    public class OnRequestMediaEventArgs : EventArgs
    {
        public IVideoControls Player { get; init; }
        public string RequestData { get; init; }
    }


    /// <summary>
    /// The panel to display when this provider is selected in the media queue menu.
    /// </summary>
    MediaProviderHeaderPanel HeaderPanel { get; }
    event EventHandler<OnRequestMediaEventArgs> OnRequestMedia;
}
