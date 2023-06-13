using Sandbox;

namespace Cinema;

public struct MediaRequest
{
    /// <summary>
    /// Information such as a YouTube ID or a URL that uniquely identifies
    /// the media being requested.
    /// </summary>
    public string RequestData { get; set; }
    public IClient Requestor { get; set; }
    
}
