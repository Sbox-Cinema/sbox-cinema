using Conna.Inventory;

namespace Cinema;

public interface IHandheldItem : IInventoryItem
{
    public string WeaponClassName { get; }
    public WeaponBase Weapon { get; }
    public WeaponBase CreateWeaponEntity();
    public void DestroyWeaponEntity();
}
