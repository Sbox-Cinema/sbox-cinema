using Editor;
using Sandbox;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    public UI.Tooltip Tooltip { get; set; }
    public UI.Placement PlacementIndicator { get; set; }

    private List<Cookable.Hotdog> HotdogsFront = new();
    private List<Cookable.Hotdog> HotdogsBack = new();

    [Net] public bool IsOn { get; set; } = false;
    public string UseText => "Press E to use Hotdog Roller";

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
    }
}
