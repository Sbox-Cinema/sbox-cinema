using Sandbox;
using System.Collections.Generic;
using System.Linq;

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

    public void Simulate()
    {
        if (Game.IsClient) return;

        foreach (var hotdog in FrontRollerHotdogs)
        {
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

        foreach (var hotdog in BackRollerHotdogs)
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
        if (Game.IsServer)
        {
            if (FrontRollerHotdogs.Count < MaxHotDogsPerRollers)
            {
                string attachment = $"S{FrontRollerHotdogs.Count + 1}F";

                var hotdog = new HotdogCookable();

                AttachEntity(attachment, hotdog);

                FrontRollerHotdogs.Add(hotdog);
            }
        }
    }

    public void RemoveFrontRollerHotdog()
    {
        if(Game.IsServer)
        {
            if (FrontRollerHotdogs.Count > 0)
            {
                FrontRollerHotdogs.ElementAt(0).Delete();
                FrontRollerHotdogs.RemoveAt(0);
            }
        }
    }
    public void AddBackRollerHotdog()
    {
        if (Game.IsServer)
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
    }

    public void RemoveBackRollerHotdog()
    {
        if (Game.IsServer)
        {
            if (BackRollerHotdogs.Count > 0)
            {
                BackRollerHotdogs.ElementAt(0).Delete();
                BackRollerHotdogs.RemoveAt(0);
            }
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
