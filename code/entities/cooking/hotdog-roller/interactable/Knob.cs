using Sandbox;
using Cinema.Interactables;

namespace Cinema.HotdogRoller;

public class Knob : BaseInteractable
{
    public int KnobRotation { get; set; } = 0;
    private string animName { get; set; }

    public Knob() // For the compiler...
    {
    }

    public Knob(string animName)
    {
        this.animName = animName;
    }

    public override void Trigger()
    {
        KnobRotation += 1;
        KnobRotation = KnobRotation > 7 ? 0 : KnobRotation;

        (Parent as AnimatedEntity).SetAnimParameter(animName, KnobRotation);

        Sound.FromEntity("knob_turn_01", Parent);
    }
}
