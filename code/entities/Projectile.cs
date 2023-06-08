using Sandbox;

namespace Cinema;

public class Projectile : Prop
{
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
        // Prevent the projectile from immediately breaking on the person who threw it.
        if (TimeSinceSpawned < 0.25f && eventData.Other.Entity == Owner)
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
}
