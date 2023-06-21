using System.Linq;
using Sandbox;

namespace Cinema;

public partial class Projectile : BasePhysics
{
    /// <summary>
    /// If true, food and drink items will automatically be removed from
    /// a player's inventory after being thrown.
    /// </summary>
    [ConVar.Replicated("fnb.thrown.autoremove")]
    public static bool AutoRemoveThrown { get; set; } = true;

    /// <summary>
    /// Defines the amount of time after a projectile is thrown by a seated player
    /// that the NoCollide hinge hack will be applied between the player and projectile.
    /// </summary>
    [ConVar.Replicated("fnb.thrown.nocollidetime")]
    public static float NoCollideTime { get; set; } = 0.25f;

    /// <summary>
    /// In cases where it makes sense for a projectile to automatically break
    /// apart after being launched, this should be set to true.
    /// </summary>
    public bool BreakAfterLaunch { get; init; } = false;
    /// <summary>
    /// Specifies the amount of time that must pass before the projectile automatically
    /// breaks from being launched. If this is set to a negative value, then the projectile
    /// will break after only a very short delay.
    /// </summary>
    public float TimeUntilBreak { get; set; } = -1f;

    /// <summary>
    /// Because we assume that a projectile is launched as soon as it is spawned,
    /// this is analogous to air time.
    /// </summary>
    protected TimeSince TimeSinceSpawned { get; set; }

    private static float VelocityToBreak => 120;

    public override void Spawn()
    {
        base.Spawn();
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);

        Tags.Add("projectile");

        // We assume that the projectile is in flight from the moment it is spawned.
        TimeSinceSpawned = 0f;
        if (TimeUntilBreak < 0f)
        {
            TimeUntilBreak = 0.05f;
        }
    }

    public void LaunchFromEntityViewpoint(Entity entity, float speedFactor = 1.0f)
    {
        Owner = entity;
        entity ??= this;

        // Use nocollide hinge hack to work around issue where popcorn collides the thrower.
        if (entity is Player)
        {
            NoCollide.BeginTimed(entity, this, NoCollideTime);
        }

        var forward = entity.AimRay.Forward;

        Position = entity.AimRay.Position + forward * 5.0f;
        Rotation = Rotation.LookAt(forward);

        var forwardVelocity = forward * 450.0f * speedFactor;
        var upwardVelocity = entity.Rotation.Up * 250.0f * speedFactor;
        PhysicsBody.Velocity = forwardVelocity + upwardVelocity;
        PhysicsBody.AngularVelocity = forward + Vector3.Random * 15;
    }

    [GameEvent.Tick.Server]
    protected void Tick()
    {
        // If projectile breaks after launch, check if it's time to do so.
        if (BreakAfterLaunch && TimeSinceSpawned >= TimeUntilBreak)
        {
            Break();
        }
    }

    protected override void OnPhysicsCollision(CollisionEventData eventData)
    {
        lastCollision = eventData;

        // Don't break on the chair you're sitting in.
        if (eventData.Other.Entity is CinemaChair chair && chair.Occupant == Owner)
            return;

        // If the projectile is moving fast enough, break it.
        if (eventData.This.PreVelocity.Length > VelocityToBreak)
        {
            Break();
            return;
        }

        // Finally, look for any other reasons this projectile might break.
        base.OnPhysicsCollision(eventData);
    }

    public void Break()
    {
        OnKilled();
        LifeState = LifeState.Dead;
        Delete();
    }

    public override void OnKilled()
    {
        if (LifeState != LifeState.Alive)
            return;

        LifeState = LifeState.Dead;

        if (LastDamage.HasTag("physics_impact"))
        {
            Velocity = lastCollision.This.PreVelocity;
        }

        DoGibs();
        Delete(); // LifeState.Dead prevents this in OnKilled

        base.OnKilled();
    }

    CollisionEventData lastCollision;

    DamageInfo LastDamage;

    private void DoGibs()
    {
        var result = new Breakables.Result();
        result.CopyParamsFrom(LastDamage);
        Breakables.Break(this, result);

        foreach (var gib in result.Props)
        {
            var garbage = new Garbage
            {
                Position = gib.Position,
                Rotation = gib.Rotation,
                Model = gib.Model,
            };

            var phys = garbage.PhysicsBody;

            if (phys != null && gib.PhysicsBody != null)
            {
                phys.Velocity = gib.PhysicsBody.Velocity;
                phys.AngularVelocity = gib.PhysicsBody.AngularVelocity;
            }

            gib.Delete();
        }
    }
}
