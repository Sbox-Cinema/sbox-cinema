using Sandbox;
using System.Collections.Generic;

namespace Cinema;

public partial class HotdogRollerRollers : EntityComponent<HotdogRoller>
{
    private int MaxHotDogsPerRollers = 10;
    private bool IsFrontRollerPowerOn => Entity.Switches.IsFrontRollerPoweredOn();
    private bool IsBackRollerPowerOn => Entity.Switches.IsBackRollerPoweredOn();
    [Net] private IList<HotdogCookable> FrontRollerHotdogs { get; set; }
    [Net] private IList<HotdogCookable> BackRollerHotdogs { get; set; }
    protected override void OnActivate()
    {
        base.OnActivate();
    }
    protected override void OnDeactivate()
    {
        base.OnDeactivate();
    }

    [GameEvent.Tick]
    private void OnTick()
    {
        if (Game.IsClient) return;
    }

    public void AddFrontRollerHotdog()
    {
        if(FrontRollerHotdogs.Count < MaxHotDogsPerRollers)
        {
            string attachment = $"S{FrontRollerHotdogs.Count + 1}F";

            var hotdog = new HotdogCookable();

            AttachEntity(attachment, hotdog);
            
            FrontRollerHotdogs.Add(hotdog);
        }
    }

    public void AddBackRollerHotdog()
    {
        if (BackRollerHotdogs.Count < MaxHotDogsPerRollers)
        {
            var attachmentIndex = MaxHotDogsPerRollers - BackRollerHotdogs.Count;

            string attachment = $"S{attachmentIndex}B";

            var hotdog = new HotdogCookable();

            AttachEntity(attachment, hotdog);

            BackRollerHotdogs.Add(hotdog);
        }
    }

    private void AttachEntity(string attach, Entity ent)
    {
        if (Entity.GetAttachment(attach) is Transform t)
        {
            Log.Info($"Attaching {attach}");

            ent.Transform = t;

            ent.Position += (Vector3.Up * 0.5f);
            ent.Position += (Vector3.Left * 0.5f);
        }
    }
}
