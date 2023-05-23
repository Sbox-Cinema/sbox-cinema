using Editor;
using Sandbox;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    [Net] public IDictionary<string, Hotdog> Hotdogs {get; set;}
}
