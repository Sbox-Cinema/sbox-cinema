namespace Cinema;

public interface IHandheldItem
{
    public string WeaponClassName { get; }
    public WeaponBase Weapon { get; }
    public WeaponBase CreateWeaponEntity();
    public void DestroyWeaponEntity();
}
