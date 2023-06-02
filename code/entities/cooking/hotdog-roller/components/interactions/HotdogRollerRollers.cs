using Sandbox;
using System.Collections.Generic;

namespace Cinema;

public partial class HotdogRollerRollers : EntityComponent<HotdogRoller>
{
    private bool IsFrontRollerPowerOn => Entity.Switches.IsFrontRollerPoweredOn();
    private bool IsBackRollerPowerOn => Entity.Switches.IsBackRollerPoweredOn();
    private Dictionary<string, HotdogCookable> Hotdogs { get; set; } = new();
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

    public void AddHotdog(int rollerid)
    {
        switch (rollerid)
        {
            case 1:
                var hotdog1 = Hotdogs.GetOrCreate<string, HotdogCookable>("S1F");
                var hotdog2 = Hotdogs.GetOrCreate<string, HotdogCookable>("S2F");

                if (hotdog1.IsValid && hotdog2.IsValid)
                {
                    AttachEntity("S1F", hotdog1);
                    AttachEntity("S2F", hotdog2);
                }

                break;
            case 2:
                var hotdog3 = Hotdogs.GetOrCreate<string, HotdogCookable>("S3F");
                var hotdog4 = Hotdogs.GetOrCreate<string, HotdogCookable>("S4F");

                if (hotdog3.IsValid && hotdog4.IsValid)
                {
                    AttachEntity("S3F", hotdog3);
                    AttachEntity("S4F", hotdog4);
                }
                break;
            case 3:
                var hotdog5 = Hotdogs.GetOrCreate<string, HotdogCookable>("S5F");
                var hotdog6 = Hotdogs.GetOrCreate<string, HotdogCookable>("S6F");

                if (hotdog5.IsValid && hotdog6.IsValid)
                {
                    AttachEntity("S5F", hotdog5);
                    AttachEntity("S6F", hotdog6);
                }
                break;
            case 4:
                var hotdog7 = Hotdogs.GetOrCreate<string, HotdogCookable>("S7F");
                var hotdog8 = Hotdogs.GetOrCreate<string, HotdogCookable>("S8F");

                if (hotdog7.IsValid && hotdog8.IsValid)
                {
                    AttachEntity("S7F", hotdog7);
                    AttachEntity("S8F", hotdog8);
                }
                break;
            case 5:
                var hotdog9 = Hotdogs.GetOrCreate<string, HotdogCookable>("S9F");
                var hotdog10 = Hotdogs.GetOrCreate<string, HotdogCookable>("S10F");

                if (hotdog9.IsValid && hotdog10.IsValid)
                {
                    AttachEntity("S9F", hotdog9);
                    AttachEntity("S10F", hotdog10);
                }
                break;
            case 6:
                var hotdog11 = Hotdogs.GetOrCreate<string, HotdogCookable>("S9B");
                var hotdog12 = Hotdogs.GetOrCreate<string, HotdogCookable>("S10B");

                if (hotdog11.IsValid && hotdog12.IsValid)
                {
                    AttachEntity("S9B", hotdog11);
                    AttachEntity("S10B", hotdog12);
                }
                break;
            case 7:
                var hotdog13 = Hotdogs.GetOrCreate<string, HotdogCookable>("S8B");
                var hotdog14 = Hotdogs.GetOrCreate<string, HotdogCookable>("S7B");

                if (hotdog13.IsValid && hotdog14.IsValid)
                {
                    AttachEntity("S8B", hotdog13);
                    AttachEntity("S7B", hotdog14);
                }
                break;
            case 8:
                var hotdog15 = Hotdogs.GetOrCreate<string, HotdogCookable>("S6B");
                var hotdog16 = Hotdogs.GetOrCreate<string, HotdogCookable>("S5B");

                if (hotdog15.IsValid && hotdog16.IsValid)
                {
                    AttachEntity("S6B", hotdog15);
                    AttachEntity("S5B", hotdog16);
                }
                break;
            case 9:
                var hotdog17 = Hotdogs.GetOrCreate<string, HotdogCookable>("S4B");
                var hotdog18 = Hotdogs.GetOrCreate<string, HotdogCookable>("S3B");

                if (hotdog17.IsValid && hotdog18.IsValid)
                {
                    AttachEntity("S4B", hotdog17);
                    AttachEntity("S3B", hotdog18);
                }
                break;
            case 10:
                var hotdog19 = Hotdogs.GetOrCreate<string, HotdogCookable>("S2B");
                var hotdog20 = Hotdogs.GetOrCreate<string, HotdogCookable>("S1B");

                if (hotdog19.IsValid && hotdog20.IsValid)
                {
                    AttachEntity("S2B", hotdog19);
                    AttachEntity("S1B", hotdog20);
                }
                break;

        }
    }

    /// <summary>
    /// Attaches hotdog by attachment point
    /// </summary>
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
