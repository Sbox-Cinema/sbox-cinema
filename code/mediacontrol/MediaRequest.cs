using Sandbox;

namespace Cinema;

public struct MediaRequest
{
    public string YouTubeId { get; set; }
    public IClient Requestor { get; set; }
}
