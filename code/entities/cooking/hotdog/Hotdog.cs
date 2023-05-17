using Sandbox;

namespace Cinema.Cookable;

public partial class Hotdog : AnimatedEntity
{
    [BindComponent] public Rotator Rotator { get; }
    [BindComponent] public Rotator Steam { get; }
}
