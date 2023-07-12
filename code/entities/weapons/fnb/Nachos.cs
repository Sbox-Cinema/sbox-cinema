using Sandbox;

namespace Cinema;

public partial class Nachos : WeaponBase
{
    public override float PrimaryFireRate => 0.65f;
    public override int BaseUses => 26;

    public override void Spawn()
    {
        base.Spawn();
        Model = Model.Load("models/hotdog/w_hotdog_boxed.vmdl");

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
                Model = Model.Load("models/nachos_tray/nachos_tray.vmdl"),
                BreakAfterLaunch = true
            };

            projectile.LaunchFromEntityViewpoint(WeaponHolder);
            
            if (Projectile.AutoRemoveThrown)
                RemoveFromHolder();
        }
    }

    public override void Reload()
    {
    }
}
