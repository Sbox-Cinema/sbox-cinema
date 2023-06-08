using Sandbox;

namespace Cinema;

public class Projectile : Prop
{
    private static float VelocityToBreak => 120;

    public override void Spawn()
    {
        base.Spawn();
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        // Remove the solid tag so that the projectile doesn't collide with the owner.
        Tags.Remove("solid");
    }

    [GameEvent.Tick.Server]
    protected void Tick()
    {

    }

    protected override void OnPhysicsCollision(CollisionEventData eventData)
    {
        base.OnPhysicsCollision(eventData);

        if (eventData.Other.Entity is not WorldEntity || eventData.Other.Entity == this) return;

        if (eventData.This.PreVelocity.Length > VelocityToBreak)
            Break();
    }
}
