using Sandbox;

namespace Cinema;

public partial class Rotator : EntityComponent
{
    [Net] private float RotatorDeg { get; set; } = 0.0f;
    [Net] private float RotatorSpeed { get; set; } = 0.5f;

    /// <summary>
    /// Called when activated 
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        RotatorDeg = Entity.Rotation.Roll();
    }

    /// <summary>
    /// Updates the Entity's rotation every tick
    /// </summary>
    [GameEvent.Tick]
    public void Update()
    {
        UpdateRotator();
        UpdateEntity();
    }

    /// <summary>
    /// Updates the component
    /// </summary>
    private void UpdateRotator()
    {
        if (!Game.IsServer) return;

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
        if (!Game.IsServer) return;

        Entity.Rotation = Rotation.FromRoll(RotatorDeg);
    }
}
