using Sandbox;

namespace Cinema;

public partial class Rotator : EntityComponent
{
    private float RotatorDegrees { get; set; } = 180.0f;
    static private float RotatorSpeed { get; set; } = 2.0f;

    /// <summary>
    /// Called when activated 
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        // Set rotator to entity's current rotation (roll)
        RotatorDegrees = Entity.Rotation.Roll();
    }

    /// <summary>
    /// Updates the Entity's rotation every tick
    /// </summary>
    [GameEvent.Tick]
    public void Update()
    {
        if(!Game.IsServer) return;

        UpdateRotator();
        UpdateEntity();
    }

    /// <summary>
    /// Updates the component
    /// </summary>
    private void UpdateRotator()
    { 
        RotatorDegrees -= RotatorSpeed;

        if (RotatorDegrees < -180.0f)
        {
            RotatorDegrees = 180.0f;
        }
    }

    /// <summary>
    /// Updates the Entity's rotation
    /// </summary>
    private void UpdateEntity()
    {
        Entity.Rotation = Rotation.From(new Angles(Entity.Rotation.Pitch(), Entity.Rotation.Yaw(), RotatorDegrees));
    }
}
