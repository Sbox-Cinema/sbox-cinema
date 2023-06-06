using Sandbox;
using System.Collections.Generic;

namespace Cinema;

public partial class HotdogRollerRollers : EntityComponent<HotdogRoller>
{
    private int MaxHotDogsPerRollers = 10;
    private bool IsFrontRollerPowerOn => Entity.Switches.IsFrontRollerPoweredOn();
    private bool IsBackRollerPowerOn => Entity.Switches.IsBackRollerPoweredOn();
    [Net] private IDictionary<string, HotdogCookable> FrontRollerHotdogs { get; set; }
    [Net] private IList<HotdogCookable> BackRollerHotdogs { get; set; }

    protected override void OnActivate()
    {
        base.OnActivate();

    }

    [GameEvent.Tick]
    private void OnTick()
    {
        if (Game.IsClient) return;

        foreach (var obj in FrontRollerHotdogs)
        {
            var attachment = obj.Key;
            var hotdog = obj.Value;

            if (IsFrontRollerPowerOn)
            {
                hotdog.Components.GetOrCreate<Cooking>();
                hotdog.Components.GetOrCreate<Rotator>();
            }
            else if (!IsFrontRollerPowerOn)
            {
                hotdog.Components.RemoveAny<Cooking>();
                hotdog.Components.RemoveAny<Rotator>();
            }
        }

        foreach(var hotdog in BackRollerHotdogs)
        {
            if (IsBackRollerPowerOn)
            {
                hotdog.Components.GetOrCreate<Cooking>();
                hotdog.Components.GetOrCreate<Rotator>();
            }
            else if (!IsBackRollerPowerOn)
            {
                hotdog.Components.RemoveAny<Cooking>();
                hotdog.Components.RemoveAny<Rotator>();
            }
        }
    }
    public void AddFrontRollerHotdog()
    {
        if(FrontRollerHotdogs.Count < MaxHotDogsPerRollers)
        {
            string attachment = $"S{FrontRollerHotdogs.Count + 1}F";

            var hotdog = new HotdogCookable();

            AttachEntity(attachment, hotdog);
            
            FrontRollerHotdogs.Add(attachment, hotdog);
        }
    }

    public void RemoveFrontRollerHotdog()
    {
        var attachment = $"S{FrontRollerHotdogs.Count + 1}F";

        if(FrontRollerHotdogs.TryGetValue(attachment, out HotdogCookable hotdog))
        {
            hotdog.Hide();
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
            ent.Transform = t;

            ent.Position += (Vector3.Up * 0.5f);
            ent.Position += (Vector3.Left * 0.5f);
        }
    }
}
