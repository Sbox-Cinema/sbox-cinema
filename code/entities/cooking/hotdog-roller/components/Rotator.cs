using Sandbox;

namespace Cinema;

public partial class Rotator : EntityComponent
{
    [Net] private float RotatorDeg { get; set; } = 0.0f;
    [Net] private float RotatorSpeed { get; set; } = 0.5f;

    [GameEvent.Tick]
    public void Update()
    {
        UpdateRotator();
        UpdateEntity();
    }
    private void UpdateRotator()
    {
        if (!Game.IsServer) return;

        RotatorDeg += RotatorSpeed;

        if (RotatorDeg > 360)
        {
            RotatorDeg = 0.0f;
        }
    }
    private void UpdateEntity()
    {
        if (!Game.IsServer) return;

        Entity.Rotation = Rotation.FromRoll(RotatorDeg);
    }
}
