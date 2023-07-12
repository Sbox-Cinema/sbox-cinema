using Sandbox;

namespace Cinema;

public partial class Popcorn : WeaponBase
{
    public override float PrimaryFireRate => 0.85f;
    public override int BaseUses => 10;

    public override void Spawn()
    {
        base.Spawn();
        WorldModel = Model.Load("models/popcorn_tub/w_popcorn_tub_01.vmdl");
        
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
                Model = Model.Load("models/popcorn_tub/w_popcorn_tub_01.vmdl"),
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
