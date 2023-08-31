using Sandbox;

namespace Cinema;

public partial class Machine : AnimatedEntity, ICinemaUse
{
    public new virtual string Name => "Machine";
    public virtual Model WorldModel => Model.Load("models/dev/tallbox.vmdl");

    public virtual string UseText => "Use";
    public virtual string CannotUseText => "Cannot use now";
    public virtual bool ShowCannotUsePrompt => false;
    public virtual bool TimedUse => false;
    public virtual float TimedUsePercentage => 0;
    public virtual bool ToggleUse => false;

    public virtual bool IsUsable(Entity user)
    {
        return false;
    }

    public virtual bool OnStopUse(Entity user)
    {
        return true;
    }

    public virtual bool OnUse(Entity user)
    {
        return false;
    }

    public override void Spawn()
    {
        Model = WorldModel;
        SetupPhysicsFromModel(PhysicsMotionType.Static, false);
    }

}
