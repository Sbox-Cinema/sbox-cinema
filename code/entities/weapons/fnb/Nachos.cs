using Sandbox;

namespace Cinema;

public partial class Nachos : WeaponBase
{
    public override string Name => "Box of Nachos";
    public override string Description => "Its nacho business";
    public override string Icon => "ui/icons/nachos.png";
    public override Model WorldModel => Model.Load("models/nachos_tray/nachos_tray.vmdl");
    public override string ViewModelPath => "models/nachos_tray/nachos_tray.vmdl";
    public override float PrimaryFireRate => 0.65f;
    public override int BaseUses => 26;

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
                Model = Model,
                Position = WeaponHolder.AimRay.Position + WeaponHolder.AimRay.Forward * 5.0f,
                Rotation = WeaponHolder.EyeRotation,
            };

            projectile.PhysicsBody.Velocity = WeaponHolder.AimRay.Forward * 450.0f + WeaponHolder.Rotation.Up * 250.0f;
            projectile.PhysicsBody.AngularVelocity = WeaponHolder.EyeRotation.Forward + Vector3.Random * 15;
        }
    }

    public override void Reload()
    {
    }
}
