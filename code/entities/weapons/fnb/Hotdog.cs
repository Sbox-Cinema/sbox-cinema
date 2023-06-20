using Sandbox;

namespace Cinema;

public partial class Hotdog : WeaponBase
{
    public override string Name => "Hotdog";
    public override string Description => "As the Germans say: 'Heißer Hund'";
    public override string Icon => "ui/icons/hotdog.png";
    public override Model WorldModel => Model.Load("models/hotdog/w_hotdog_boxed.vmdl");
    public override string ViewModelPath => "models/hotdog/v_hotdog_boxed.vmdl";
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
                Model = Model.Load("models/hotdog/w_hotdog_boxed.vmdl")
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
