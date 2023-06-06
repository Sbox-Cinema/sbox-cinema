using Sandbox;

namespace Cinema;

/// <summary>
/// A piece of trash that can be picked up and thrown away.
/// </summary>
public partial class Trash : AnimatedEntity, ICinemaUse
{
    new public virtual string Name => "Trash";
    public virtual string Description => "A piece of trash";
    public virtual string Icon => "";
    public virtual string ModelPath => null;
    public string UseText => "Pickup";


    public override void Spawn()
    {
        base.Spawn();

        if (ModelPath != null)
        {
            SetModel(ModelPath);
        }
    }

    public bool IsUsable(Entity user)
    {
        if (user is not Player ply)
            return false;
        return ply.Job.HasAbility(Jobs.JobAbilities.PickupTrash);
    }

    public bool OnUse(Entity user)
    {
        if (user is not Player ply)
            return false;
        return false;
    }

    public void OnStopUse(Entity user)
    {
    }
}
