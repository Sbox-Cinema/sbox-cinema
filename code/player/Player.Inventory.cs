
using Conna.Inventory;
using Sandbox;

namespace Cinema;

public partial class Player
{
    [Net]
    private NetInventoryContainer InternalInventory { get; set; }
    public InventoryContainer Inventory => InternalInventory.Value;

    private void SetupInventory()
    {
        var inventory = new InventoryContainer();
        inventory.SetEntity(this);
        InventorySystem.Register(inventory);
        InternalInventory = new NetInventoryContainer(inventory);
    }
}
