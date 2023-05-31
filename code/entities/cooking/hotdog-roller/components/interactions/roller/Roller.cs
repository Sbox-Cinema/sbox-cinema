using Sandbox;
using System.Collections.Generic;

namespace Cinema;

public partial class Roller : EntityComponent<HotdogRoller>
{
    private Dictionary<string, Hotdog> Hotdogs { get; set; } = new();

    /// <summary>
    /// Called when this component is activated
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        TransitionStateTo(State.BothOff);
    }

    /// <summary>
    /// Sets the roller's state
    /// </summary>
    public void SetPos(int pos)
    {
        State state = (State) pos;

        TransitionStateTo(state);
    }

    /// <summary>
    /// Adds hotdog by roller id
    /// </summary>
    public void AddHotdog(int rollerid)
    {
        switch(rollerid)
        {
            case 1:
                var hotdog1 = Hotdogs.GetOrCreate<string, Hotdog>("S1F");
                var hotdog2 = Hotdogs.GetOrCreate<string, Hotdog>("S2F");

                if (hotdog1.IsValid && hotdog2.IsValid)
                {
                    AttachEntity("S1F", hotdog1);
                    AttachEntity("S2F", hotdog2);
                }

                break;
            case 2:
                var hotdog3 = Hotdogs.GetOrCreate<string, Hotdog>("S3F");
                var hotdog4 = Hotdogs.GetOrCreate<string, Hotdog>("S4F");

                if (hotdog3.IsValid && hotdog4.IsValid)
                {
                    AttachEntity("S3F", hotdog3);
                    AttachEntity("S4F", hotdog4);
                }
                break;
            case 3:
                var hotdog5 = Hotdogs.GetOrCreate<string, Hotdog>("S5F");
                var hotdog6 = Hotdogs.GetOrCreate<string, Hotdog>("S6F");

                if (hotdog5.IsValid && hotdog6.IsValid)
                {
                    AttachEntity("S5F", hotdog5);
                    AttachEntity("S6F", hotdog6);
                }
                break;
            case 4:
                var hotdog7 = Hotdogs.GetOrCreate<string, Hotdog>("S7F");
                var hotdog8 = Hotdogs.GetOrCreate<string, Hotdog>("S8F");

                if (hotdog7.IsValid && hotdog8.IsValid)
                {
                    AttachEntity("S7F", hotdog7);
                    AttachEntity("S8F", hotdog8);
                }
                break;
            case 5:
                var hotdog9 = Hotdogs.GetOrCreate<string, Hotdog>("S9F");
                var hotdog10 = Hotdogs.GetOrCreate<string, Hotdog>("S10F");

                if (hotdog9.IsValid && hotdog10.IsValid)
                {
                    AttachEntity("S9F", hotdog9);
                    AttachEntity("S10F", hotdog10);
                }
                break;
            case 6:
                var hotdog11 = Hotdogs.GetOrCreate<string, Hotdog>("S9B");
                var hotdog12 = Hotdogs.GetOrCreate<string, Hotdog>("S10B");

                if (hotdog11.IsValid && hotdog12.IsValid)
                {
                    AttachEntity("S9B", hotdog11);
                    AttachEntity("S10B", hotdog12);
                }
                break;
            case 7:
                var hotdog13 = Hotdogs.GetOrCreate<string, Hotdog>("S8B");
                var hotdog14 = Hotdogs.GetOrCreate<string, Hotdog>("S7B");

                if (hotdog13.IsValid && hotdog14.IsValid)
                {
                    AttachEntity("S8B", hotdog13);
                    AttachEntity("S7B", hotdog14);
                }
                break;
            case 8:
                var hotdog15 = Hotdogs.GetOrCreate<string, Hotdog>("S6B");
                var hotdog16 = Hotdogs.GetOrCreate<string, Hotdog>("S5B");

                if (hotdog15.IsValid && hotdog16.IsValid)
                {
                    AttachEntity("S6B", hotdog15);
                    AttachEntity("S5B", hotdog16);
                }
                break;
            case 9:
                var hotdog17 = Hotdogs.GetOrCreate<string, Hotdog>("S4B");
                var hotdog18 = Hotdogs.GetOrCreate<string, Hotdog>("S3B");

                if (hotdog17.IsValid && hotdog18.IsValid)
                {
                    AttachEntity("S4B", hotdog17);
                    AttachEntity("S3B", hotdog18);
                }
                break;
            case 10:
                var hotdog19 = Hotdogs.GetOrCreate<string, Hotdog>("S2B");
                var hotdog20 = Hotdogs.GetOrCreate<string, Hotdog>("S1B");

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
