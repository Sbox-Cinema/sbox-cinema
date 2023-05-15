using System.Collections.Generic;
using Sandbox;

namespace Cinema;

public partial class MediaManager : Entity
{
    public List<MediaController> Controllers { get; protected set; } = new();

    public void RegisterController(MediaController controller)
    {
        Controllers.Add(controller);
    }
}
