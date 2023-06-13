using Conna.Inventory;

namespace Cinema;

public class HandheldItem : ResourceItem<HandheldResource, HandheldItem>, IHandheldItem
{
    public virtual string WeaponClassName => Resource?.ClassName ?? string.Empty;

    public WeaponBase Weapon
    {
        get => InternalWeapon;
        set
        {
            if (InternalWeapon != value)
            {
                InternalWeapon = value;
            }
        }
    }

    private WeaponBase InternalWeapon;

    public WeaponBase CreateWeaponEntity()
    {
        if (Weapon.IsValid)
            return Weapon;

        Weapon = TypeLibrary.Create<WeaponBase>(WeaponClassName);
        Weapon.SetWeaponItem(this);
        IsDirty = true;

        return Weapon;
    }

    public void DestroyWeaponEntity()
    {
        if (!Weapon.IsValid)
            return;

        Weapon.Delete();
        Weapon = null;
        IsDirty = true;
    }
}
