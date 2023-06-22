using Sandbox;
using Sandbox.util;

namespace Cinema.HotdogRoller;

public class Knob : BaseInteractable
{
    public int KnobRotation { get; set; } = 0;
    private string AnimName { get; set; }

    public Knob() // For the compiler...
    {
    }

    public Knob(string animName)
    {
        AnimName = animName;
    }

    public override void Trigger(Player ply)
    {
        KnobRotation += 1;
        KnobRotation = KnobRotation > 7 ? 0 : KnobRotation;

        (Parent as AnimatedEntity).SetAnimParameter(AnimName, KnobRotation);

        Sound.FromEntity("knob_turn_01", Parent);
    }
}
