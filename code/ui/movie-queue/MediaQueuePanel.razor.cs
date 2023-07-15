using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class MediaQueuePanel : Panel
{
    public MediaQueue MediaQueue { get; set; }

    public string IsEmptyClass
    {
        get 
        {
            if (MediaQueue == null)
                return "empty";

            return MediaQueue.Items.Count == 0 ? "empty" : "";
        }
    }

    protected override int BuildHash()
    {
        var hashCode = 0;
        if (MediaQueue == null)
        {
            return 0;
        }
        foreach (var item in MediaQueue.Items)
        {
            hashCode = HashCode.Combine(hashCode, item.GetHashCode());
        }
        return hashCode;
    }
}
