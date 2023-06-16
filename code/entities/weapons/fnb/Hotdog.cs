using Sandbox;

namespace Cinema;

public partial class Hotdog : WeaponBase
{
    public override string Name => "Hotdog";
    public override string Description => "A raw hotdog";
    public override string Icon => "ui/icons/hotdog.png";
    public override Model WorldModel => Model.Load("models/hotdog/hotdog_roller.vmdl");
    public override string ViewModelPath => "models/hotdog/v_hotdog_roller.vmdl";
    public override float PrimaryFireRate => 1.35f;
    public override int BaseUses => 4;

    public override void Spawn()
    {
        base.Spawn();
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();

        PlaySound("placeholder_eating");
    }

    public override void SecondaryFire()
    {
        if (Game.IsClient) return;

        using (Prediction.Off())
        {
            var projectile = new Projectile()
            {
                Owner = WeaponHolder,
                Model = Model.Load("models/hotdog/hotdog_roller.vmdl"),
                Position = WeaponHolder.AimRay.Position + WeaponHolder.AimRay.Forward * 5.0f,
                Rotation = WeaponHolder.EyeRotation,
            };

            projectile.LaunchFromEntityViewpoint(WeaponHolder);
            
            RemoveFromHolder();
        }
    }

    public override void Reload()
    {
    }
}
