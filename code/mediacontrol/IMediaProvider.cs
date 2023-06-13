using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public interface IMediaProvider
{
    public class OnRequestMediaEventArgs : EventArgs
    {
        public string RequestData { get; set; }
    }
    string ProviderName { get; }

    // TODO: Add a thumbnail texture.

    /// <summary>
    /// The panel to display when this provider is selected in the media queue menu.
    /// </summary>
    MediaProviderHeaderPanel HeaderPanel { get; }

    event EventHandler<OnRequestMediaEventArgs> OnRequestMedia;
    void Play(string RequestData);
    void Pause();
    void Stop();
    void Seek(float Time);
}
