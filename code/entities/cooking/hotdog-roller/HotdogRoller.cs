using Editor;
using Sandbox;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("monitor")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
[SupportsSolid]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    public UI.Tooltip Tooltip { get; set; }

    private List<Cookable.Hotdog> HotdogsFront = new();
    private List<Cookable.Hotdog> HotdogsBack = new();

    private bool IsOn = false;

    public string UseText => "Press E to use Hotdog Roller";

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
