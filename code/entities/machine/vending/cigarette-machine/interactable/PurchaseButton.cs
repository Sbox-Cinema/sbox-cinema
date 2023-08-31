using Sandbox;
using Sandbox.util;

namespace Cinema;

public partial class PurchaseButton : BaseInteractable
{
    public string UseText => "Buy Cigarette Pack";

    public CigaretteMachine.CigarettePackType CigarettePackType { get; set; }

    [Net] public int NumDispensed { get; set; } = 0;

    public PurchaseButton() // For the compiler...
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    public override void Trigger(Player player)
    {
        var position = (Parent as AnimatedEntity).GetAttachment("Box_Spawner").Value.Position;

        var ent = new CigarettePackEntity
        {
            Position = position,
            Rotation = Rotation.FromPitch(90.0f),
            Model = Model.Load("models/cigarettepack/cigarettepack.vmdl"),
            Scale = 0.35f,
            CigarettePackType = CigarettePackType
        };
        
        ent.Initialize();

        var dispenseForce = 100.0f;

        if (NumDispensed > 5)
        {
            dispenseForce = 500.0f;

            NumDispensed = 0;
        }

        ent.ApplyAbsoluteImpulse(Vector3.Forward * dispenseForce);
        ent.ApplyLocalAngularImpulse(Vector3.Random * 1000f);

        NumDispensed++;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Simulate()
    {
        base.Simulate();
    }

}
