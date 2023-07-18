using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_service_bell"), HammerEntity]
[Title("Service Bell"), Category("Gameplay"), Icon("notifications")]
[EditorModel("models/servicebell/servicebell.vmdl")]
public partial class ServiceBell : AnimatedEntity, ICinemaUse
{

    /// <summary>
    /// Set up the model when spawned by the server
    /// Setup model
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/servicebell/servicebell.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }
}
