using Sandbox;

namespace Cinema;

public partial class Rotator : EntityComponent
{
    private float RotatorDeg { get; set; } = 0.0f;
    static private float RotatorSpeed { get; set; } = 1.0f;

    /// <summary>
    /// Called when activated 
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        // Set rotator to entity's current rotation (roll)
        RotatorDeg = Entity.Rotation.Roll();
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
        RotatorDeg += RotatorSpeed;

        if (RotatorDeg > 360)
        {
            RotatorDeg = 0.0f;
        }
    }

    /// <summary>
    /// Updates the Entity's rotation
    /// </summary>
    private void UpdateEntity()
    {
        Entity.Rotation = Rotation.FromRoll(RotatorDeg);
    }
}
