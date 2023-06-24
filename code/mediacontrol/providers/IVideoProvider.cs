using System;
namespace Cinema;

public interface IVideoProvider
{
    public class OnRequestMediaEventArgs : EventArgs
    {
        public IVideoPlayer Player { get; init; }
        public string RequestData { get; init; }
    }
    string ProviderName { get; }

    // TODO: Add a thumbnail texture.

    /// <summary>
    /// The panel to display when this provider is selected in the media queue menu.
    /// </summary>
    MediaProviderHeaderPanel HeaderPanel { get; }
    IVideoPlayer Play(string requestData);

    event EventHandler<OnRequestMediaEventArgs> OnRequestMedia;
}
