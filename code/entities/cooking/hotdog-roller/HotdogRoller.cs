using Editor;
using Sandbox;
using System;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("monitor")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
[SupportsSolid]
public partial class HotdogRoller : AnimatedEntity, IUse
{
    public UI.Tooltip Tooltip { get; set; }

    List<Cookable.Hotdog> HotdogsFront = new();
    List<Cookable.Hotdog> HotdogsBack = new();

    private float RotatorDeg = 0;
    private float RotatorSpeed = 0.5f;
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

    

    [GameEvent.Tick]
    public void Update()
    {
        UpdateRotator();
        
        foreach(var hotdog in HotdogsFront) 
        {
            hotdog.Rotation = Rotation.FromRoll(RotatorDeg);
        }

        foreach (var hotdog in HotdogsBack)
        {
            hotdog.Rotation = Rotation.FromRoll(RotatorDeg);
        }
    }

    private void UpdateRotator()
    {
        RotatorDeg += RotatorSpeed;

        if (RotatorDeg > 360)
        {
            RotatorDeg = 0.0f;
        }
    }


}
