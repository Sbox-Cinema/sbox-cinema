using Sandbox;

namespace Cinema;

public class FoodBase : WeaponBase
{
    public virtual float NutritionGain { get; }
    public virtual float HydrationGain { get; }
    public float NutritionPerUse => NutritionGain / BaseUses;
    public float HydrationPerUse => HydrationGain / BaseUses;


    public override bool CanFirePrimary()
    {
        if (!base.CanFirePrimary())
            return false;

        var hydration = WeaponHolder.Components.Get<Hydration>();
        var postUseHydration = hydration.HydrationLevel + HydrationPerUse;
        // If using this item would cause the player to have very low hydration, don't allow it.
        if (postUseHydration <= 0)
        {
            return false;
        }
        var nutrition = WeaponHolder.Components.Get<Nutrition>();
        var postUseNutrition = nutrition.NutritionLevel + NutritionPerUse;
        var wouldOvereat = nutrition.NutritionLevel == nutrition.MaxNutritionLevel && NutritionPerUse > 0;
        // Don't allow the player to eat if they're already full or would starve by eating this.
        if (wouldOvereat || postUseNutrition <= 0)
        {
            return false;
        }
        return true;
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();

        
        PlaySound("placeholder_eating");
        WeaponHolder.Components.Get<Stomach>().Ingest(this);

        if (UsesRemaining <= 0)
        {
            // TODO: Turn this food in to garbage.
            RemoveFromHolder();
        }
    }

    public override void SecondaryFire()
    {
        if (Game.IsClient) return;

        using (Prediction.Off())
        {
            var projectile = new Projectile()
            {
                Owner = WeaponHolder,
                Model = WorldModel,
                Position = WeaponHolder.AimRay.Position + WeaponHolder.AimRay.Forward * 5.0f,
                Rotation = WeaponHolder.EyeRotation,
            };

            projectile.PhysicsBody.Velocity = WeaponHolder.AimRay.Forward * 450.0f + WeaponHolder.Rotation.Up * 250.0f;
            projectile.PhysicsBody.AngularVelocity = WeaponHolder.EyeRotation.Forward + Vector3.Random * 15;

            RemoveFromHolder();
        }
    }

    public override void Reload()
    {
    }
}
