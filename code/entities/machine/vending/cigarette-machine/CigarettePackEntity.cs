using Sandbox;
using Conna.Inventory;

namespace Cinema;

public partial class CigarettePackEntity : ModelEntity, IUse
{
    public bool IsUsable(Entity user)
    {
        return true;
    }

    public bool OnUse(Entity user)
    {
        if(user is Player player)
        {
            player.PickupItem(InventorySystem.CreateItem("cigarettepack-strikeforce"));
        }

        this.Delete();

        return false;
    }

    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }
}
