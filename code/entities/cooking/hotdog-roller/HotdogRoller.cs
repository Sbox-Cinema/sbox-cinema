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

    public string UseText => "Press E to use Hotdog Roller";

    [Net] public IList<Cookable.Hotdog> HotdogsFront { get; set; }
    [Net] public IList<Cookable.Hotdog> HotdogsBack { get; set; }
    [Net, Change] public bool IsOn { get; set; }

    public void OnIsOnChanged(bool oldValue, bool newValue)
    {
        
    }    
}
