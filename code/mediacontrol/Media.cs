using Sandbox;

namespace Cinema;

public partial class Media : BaseNetworkable
{
    [Net]
    public string Url { get; set; }

    [Net]
    public IClient Requestor { get; set; }
}
