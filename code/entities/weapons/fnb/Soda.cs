using Sandbox;

namespace Cinema;

public partial class Soda : WeaponBase
{
    public override string Name => "Cup of Soda";
    public override string Description => "A delicious taste of soda";
    public override string Icon => "ui/icons/soda.png";
    public override Model WorldModel => Model.Load("models/papercup/papercup.vmdl");
    public override string ViewModelPath => "models/papercup/v_papercup.vmdl";
    public override float PrimaryFireRate => 1.35f;
    public override int BaseUses => 8;
    public override ActionEnum InputActionType => ActionEnum.HoldToUse;
    public override float PerHeldInterval => 2.0f;

    public override void Spawn()
    {
        base.Spawn();
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);

        AnimateAttack(false, IsHolding);
    }

    public override void DoFireHolding()
    {
        if (IsStillHolding())
        {
            if (HeldFire >= PerHeldInterval)
            {
                UsesRemaining--;
                PlayFireSounds(IsAltHold);
                HeldFire = 0.0f;
            }
        }
        else
        {
            AnimateAttack(IsAltHold, false);

            IsHolding = false;
            IsAltHold = false;
        }
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();
    }

    public override void SecondaryFire()
    {
        if (Game.IsClient) return;

        AnimateAttack(true);

        using (Prediction.Off())
        {
            var projectile = new Projectile()
            {
                Owner = WeaponHolder,
                Model = Model.Load("models/papercup/papercup.vmdl"),
                Position = WeaponHolder.AimRay.Position + WeaponHolder.AimRay.Forward * 5.0f,
                Rotation = WeaponHolder.EyeRotation,
            };

            projectile.PhysicsBody.Velocity = WeaponHolder.AimRay.Forward * 450.0f + WeaponHolder.Rotation.Up * 250.0f;
            projectile.PhysicsBody.AngularVelocity = WeaponHolder.EyeRotation.Forward + Vector3.Random * 15;
        }
    }

    [ClientRpc]
    public override void AnimateAttack(bool isAlt = false, bool force = false)
    {
        if (!isAlt)
            ViewModelEntity?.SetAnimParameter("use", force);
        else
            ViewModelEntity?.SetAnimParameter("throw", true);
    }

    public override void Reload()
    {
    }
}
