using Sandbox;
using Cinema.Interactables;

namespace Cinema.HotdogRoller;

public class Knob : BaseInteractable
{
    private string animName { get; set; }
    private int knobRotation { get; set; } = 0;

    public Knob() // For the compiler...
    {
    }

    public Knob(string animName)
    {
        this.animName = animName;
    }

    public override void Trigger()
    {
        knobRotation += 1;
        knobRotation = knobRotation > 7 ? 0 : knobRotation;

        (Parent as AnimatedEntity).SetAnimParameter(animName, knobRotation);

        Sound.FromEntity("knob_turn_01", Parent);
    }
}
