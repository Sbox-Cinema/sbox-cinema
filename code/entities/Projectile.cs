using Sandbox;
using System;

namespace Cinema;

public class Projectile : ModelEntity
{
    private static float VelocityToBreak => 120;

    static float minZBreak => 0.75f;

    public override void Spawn()
    {
        base.Spawn();
        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }

    [GameEvent.Tick.Server]
    protected void Tick()
    {

    }

    protected override void OnPhysicsCollision(CollisionEventData eventData)
    {
        base.OnPhysicsCollision(eventData);

        if (eventData.Other.Entity is not WorldEntity || eventData.Other.Entity == this) return;

        if (Math.Abs(eventData.Normal.z) >= minZBreak)
        {
            var trash = new Trash()
            {
                Model = this.Model,
            };

            trash.SetUp(Position);

            Delete();
        }
    }
}
