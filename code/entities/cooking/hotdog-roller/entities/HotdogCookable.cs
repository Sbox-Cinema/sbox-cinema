using Sandbox;

namespace Cinema;

public partial class HotdogCookable : AnimatedEntity
{
    [BindComponent] public Rotator Rotator { get; }
    [BindComponent] public Rotator Steam { get; }

    /// <summary>
    /// 
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();
        
        Tags.Add("cookable");
    }
    /// <summary>
    /// 
    /// </summary>
    public override void ClientSpawn()
    {
        base.ClientSpawn();
    }
    /// <summary>
    /// 
    /// </summary>
    private void SetupModel()
    {
        Transmit = TransmitType.Always;

        SetModel("models/hotdog/hotdog_roller.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Show()
    {
        EnableDrawing = true;
    }
    /// <summary>
    /// 
    /// </summary>
    public void Hide()
    {
        EnableDrawing = false;
    }
}
