using Sandbox;

namespace Cinema;

public partial class Soda : WeaponBase
{
    public override float PrimaryFireRate => 1.35f;
    public override int BaseUses => 8;

    public override void Spawn()
    {
        base.Spawn();
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();

        PlaySound("placeholder_drinking");
    }

    public override void SecondaryFire()
    {
        if (Game.IsClient) return;

        using (Prediction.Off())
        {
            var projectile = new Projectile()
            {
                Model = Model.Load("models/papercup/papercup.vmdl"),
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
