using Sandbox;
using Conna.Inventory;

namespace Cinema;

public partial class CigarettePackEntity : ModelEntity, ICinemaUse
{
    public string UseText { get; set; } = "Pickup";

    [Net] public CigaretteMachine.CigarettePackType CigarettePackType {get; set;}
    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }

    public void Initialize()
    {
        Log.Info($"Initializing {CigarettePackType}");

        SetMaterialGroup((int)CigarettePackType);
    }

    public bool IsUsable(Entity user)
    {
        return true;
    }
    public bool OnUse(Entity user)
    {
        if (user is Player player)
        {
            player.PickupItem(InventorySystem.CreateItem(CigaretteMachine.GetCigaretteItemIdByCigarettePackTypType(CigarettePackType)));
        }

        this.Delete();

        return false;
    }
    public void OnStopUse(Entity user)
    {

    }
}
