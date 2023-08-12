
using System;
using System.Collections.Generic;
using System.Linq;
using Conna.Inventory;
using Sandbox;

namespace Cinema;

public partial class Player
{
    [Net]
    private NetInventoryContainer InternalInventory { get; set; }
    public InventoryContainer Inventory => InternalInventory.Value;

    public List<IHandheldItem> HandheldItems => Inventory.ItemList.OfType<IHandheldItem>().ToList();

    public List<WeaponBase> Weapons => HandheldItems.Select(e => e.Weapon).ToList();

    public bool HasInventorySpace => Inventory.FindFreeSlot(out var _) == true;

    public bool PickupItem(IInventoryItem item)
    {
        return Inventory.Give(item);
    }

    private void SetupInventory()
    {
        var inventory = new InventoryContainer();
        inventory.SetEntity(this);
        inventory.SetSlotLimit(32);
        inventory.ItemGiven += OnInventoryItemGiven;
        inventory.ItemTaken += OnInventoryItemTaken;
        InventorySystem.Register(inventory);
        InternalInventory = new NetInventoryContainer(inventory);
    }

    private void OnInventoryItemGiven(ushort slot, IInventoryItem instance)
    {
        if (instance is IHandheldItem handheldItem)
        {
            InitializeHandheld(handheldItem);
        }
    }

    private void OnInventoryItemTaken(ushort slot, IInventoryItem instance)
    {
        if (instance is not IHandheldItem weapon)
        {
            Log.Info("Not Handheld Item");
            return;
        }

        if (!weapon.Weapon.IsValid())
        {
            Log.Info("Not valid isWeapon Item");
            return;
        }

        if (ActiveChild == weapon.Weapon)
        {
            ActiveChild = GetBestWeapon();
        }

        weapon.DestroyWeaponEntity();
    }

    public WeaponBase GetBestWeapon()
    {
        return Weapons.FirstOrDefault();
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
