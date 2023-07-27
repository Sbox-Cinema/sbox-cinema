using Sandbox;
using System.Linq;

namespace Cinema;

public partial class PlayerController : EntityComponent<Player>
{
    public bool Active
    {
        get { return _Active; }
        set
        {
            Entity.PlayerControllers.ToList().ForEach((e) => e._Active = false);
            _Active = value;
        }
    }

    [Net]
    private bool _Active { get; set; } = false;

    public virtual void Simulate(IClient cl) { }

    public virtual void FrameSimulate(IClient cl) { }
}
