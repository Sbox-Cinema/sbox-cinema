using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_beach_ball"), HammerEntity]
[Title("Beach Ball"), Category("Gameplay"), Icon("attractions")]
[EditorModel("models/citizen_props/beachball.vmdl")]
public partial class BeachBall : AnimatedEntity, ICinemaUse
{
    /// <summary>
    /// Set up the model when spawned by the server
    /// Setup model
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        Model = Cloud.Model("garry.beachball");

        SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
    }
}
