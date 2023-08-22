using Sandbox;
using Conna.Inventory;

namespace Cinema;

public partial class CigarettePackEntity : ModelEntity, ICinemaUse
{
    public string UseText { get; set; } = "Pickup";

    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }
    public bool IsUsable(Entity user)
    {
        return true;
    }
    public bool OnUse(Entity user)
    {
        if (user is Player player)
        {
            player.PickupItem(InventorySystem.CreateItem("cigarettepack-strikeforce"));
        }

        this.Delete();

        return false;
    }
    public void OnStopUse(Entity user)
    {

    }
}
