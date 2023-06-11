using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public abstract class BaseNeed : EntityComponent<Player>, INeedInfo
{
    public BaseNeed()
    {
        NeedOscillationPhase = Game.Random.Float(0, 10);
        NeedOscillationSpeedFactor = Game.Random.Float(0.2f, 1.5f);
    }

    public abstract string DisplayName { get; }
    public abstract string IconPath { get; }
    protected float NeedOscillationPhase { get; set; }
    protected float NeedOscillationSpeedFactor { get; set; }
    public virtual float SatisfactionPercent
    {
        get
        {
            var amount = (MathF.Sin((Time.Now + NeedOscillationPhase) * NeedOscillationSpeedFactor) + 1);
            return amount * 100;
        }
    }

    protected override void OnActivate()
    {
        Log.Info($"{Game.LocalClient} - Added need: {DisplayName}");
    }
}
