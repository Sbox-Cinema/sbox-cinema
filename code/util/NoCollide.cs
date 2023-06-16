using Sandbox;
using Sandbox.Physics;

namespace Cinema;

/// <summary>
/// Allows for two entities to be made to not collide with each other.
/// </summary>
public static class NoCollide
{
    /// <summary>
    /// Applies nocollide to the two specified entities by calling <c>Begin</c> and 
    /// then calling <c>End</c> after the specified <paramref name="lifetimeInSeconds"/> elapses.
    /// </summary>
    /// <param name="first">An entity that shall be made to not collide with <paramref name="second"/>.</param>
    /// <param name="second">An entity that shall be made to not collide with <paramref name="first"/>.</param>
    /// <param name="lifetimeInSeconds">
    /// The amount of time in seconds that will elapse before the nocollide constraint will automatically
    /// be removed.
    /// </param>
    public static void BeginTimed(Entity first, Entity second, float lifetimeInSeconds)
    {
        if (lifetimeInSeconds <= 0f)
            lifetimeInSeconds = Time.Delta;

        var handle = Begin(first, second);
        if (handle == null)
            return;
        
        GameTask.RunInThreadAsync(async () =>
        {
            await GameTask.DelaySeconds(lifetimeInSeconds);
            End(handle);
        });
    }

    /// <summary>
    /// Applies nocollide to the two specified entities. Returns a handle that can be used to
    /// call <c>End</c> and allow the entities to collide once more.
    /// </summary>
    /// <param name="first">An entity that shall be made to not collide with <paramref name="second"/>.</param>
    /// <param name="second">An entity that shall be made to not collide with <paramref name="first"/>.</param>
    public static object Begin(Entity first, Entity second)
    {
        if (first.PhysicsGroup.BodyCount <= 0 || second.PhysicsGroup.BodyCount <= 0)
        {
            Log.Info($"{nameof(NoCollide)}: Entity has no physics body");
            return null;
        }
            
        var firstBody = first.PhysicsGroup.GetBody(0);
        var secondBody = second.PhysicsGroup.GetBody(0);
        var firstPoint = PhysicsPoint.Local(firstBody);
        var secondPoint = PhysicsPoint.Local(secondBody);
        return PhysicsJoint.CreateLength(firstPoint, secondPoint, float.PositiveInfinity);
    }  
    
    /// <summary>
    /// Ends the nocollide constraint between the two entities.
    /// </summary>
    /// <param name="handle">The handle that was returned by a call to <c>Begin</c>.</param>
    public static void End(object handle)
    {
        if (handle is not PhysicsJoint physicsJoint)
        {
            Log.Info($"{nameof(NoCollide)}: {nameof(handle)} is not a PhysicsJoint");
            return;
        }
        // Make sure these entities exist first before ending the no collide.
        if (!physicsJoint.Body1.GetEntity().IsValid() || !physicsJoint.Body2.GetEntity().IsValid())
        {
            return;
        }
        physicsJoint.Remove();
    }
}
