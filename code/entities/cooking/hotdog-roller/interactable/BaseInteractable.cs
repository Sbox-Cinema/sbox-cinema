using Sandbox;

namespace Cinema.Interactables;

public partial class BaseInteractable : BaseNetworkable
{
    public Vector3 Mins { get; set; }
    public Vector3 Maxs { get; set; }

    public BaseInteractable()
    {

    }

    public virtual void Trigger()
    {

    }

    public bool TryTrigger( Vector3 pos )
    {


        return false;
    }

    public virtual void Tick()
    {

    }
       

}
