
using System;
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
        inventory.ItemGiven += OnInventoryItemGiven;
        inventory.ItemTaken += OnInventoryItemTaken;
        InventorySystem.Register(inventory);
        InternalInventory = new NetInventoryContainer(inventory);
    }

    private void OnInventoryItemGiven(ushort slot, InventoryItem instance)
    {
        if (instance is IHandheldItem handheldItem)
        {
            InitializeHandheld(handheldItem);
        }
    }

    private void OnInventoryItemTaken(ushort slot, InventoryItem instance)
    {
        if (instance is not IHandheldItem weapon)
        {
            return;
        }

        if (!weapon.Weapon.IsValid())
        {
            return;
        }

        weapon.DestroyWeaponEntity();
    }

    private void InitializeHandheld(IHandheldItem item)
    {
        if (item.Weapon.IsValid()) return;
        try
        {
            var weapon = item.CreateWeaponEntity();
            weapon.OnCarryStart(this);

            if (ActiveChild == null && Game.IsServer)
            {
                ActiveChild = weapon;
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }
}
