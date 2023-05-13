using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("monitor")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
[SupportsSolid]
public partial class HotdogRoller : AnimatedEntity, IUse
{
    public UI.Tooltip Tooltip { get; set; }
   
    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);

        if (Game.IsServer)
        {
            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Use))
                {
                    Log.Info("turn off??");
                }
            }
        }
    }
}
