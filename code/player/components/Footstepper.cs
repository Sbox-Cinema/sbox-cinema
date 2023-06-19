using Sandbox;

namespace Cinema;

public partial class Footstepper : EntityComponent<AnimatedEntity>, ISingletonComponent
{
    /// <summary>
    /// The final volume of a footstep sound will be scaled by this value.
    /// </summary>
    [Net]
    public float VolumeScale { get; set; } = 15f;

    protected TimeSince TimeSinceLastFootstep { get; private set; } = 0;

    /// <summary>
    /// Using the same arguments available to <c>Player.OnAnimEventFootstep</c>, this method
    /// will play a footstep sound at the given position. The footstep sound that is played
    /// will be determined by the surface that the player is standing on.
    /// </summary>
    public virtual void DoFootstep(Vector3 pos, int foot, float volume)
    {
        if (Entity.LifeState != LifeState.Alive)
            return;

        if (!Game.IsClient)
            return;

        if (TimeSinceLastFootstep < 0.2f)
            return;

        volume *= FootstepVolume() * VolumeScale;

        TimeSinceLastFootstep = 0;

        var tr = Trace.Ray(pos, pos + Vector3.Down * 20)
            .Radius(1)
            .Ignore(Entity)
            .Run();

        if (!tr.Hit) return;

        tr.Surface.DoFootstep(Entity, tr, foot, volume);
    }

    /// <summary>
    /// Maps the velocity of the <c>Entity</c> to a percent of maximum footstep volume.
    /// </summary>
    /// <returns>The footstep volume, where 1 is full volume.</returns>
    public virtual float FootstepVolume()
    {
        return Entity.Velocity.WithZ(0).Length.LerpInverse(0.0f, 200.0f);
    }
}
