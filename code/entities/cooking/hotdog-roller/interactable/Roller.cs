using Sandbox;
using Cinema.Interactables;

namespace Cinema.HotdogRoller;

public class Roller : BaseInteractable
{
    private struct Slot
    {
        public Entity Entity { get; set; }
        public string Attachment { get; set; }

        public Slot(string attachment)
        {
            Attachment = attachment;
        }
    }

    private Switch powerSwitch { get; set; }
    private bool front;
    private Slot[] slots = new Slot[10];

    public Roller() // For the compiler...
    {
    }

    public Roller(Switch p_switch, bool front = false)
    {
        powerSwitch = p_switch;
        this.front = front;

        for (int i = 0; i < 10; i++)
        {
            slots[i] = new Slot($"S{i + 1}{(front ? 'F' : 'B')}");
        }
    }

    private void addHotdog()
    {
        for (int i = 0; i < 10; i++)
        {
            var index = !front ? 9 - i : i;
            var slotData = slots[index];

            if (slotData.Entity is null || !slotData.Entity.IsValid)
            {
                var parent = Parent as HotdogRoller;
                var hotdog = new HotdogCookable(powerSwitch);

                if (parent.GetAttachment(slotData.Attachment) is Transform t)
                {
                    hotdog.Transform = t;
                }
                slotData.Entity = hotdog;

                var reverse = Game.Random.Int(1) == 1;

                hotdog.LocalRotation = hotdog.LocalRotation.RotateAroundAxis(Vector3.Forward, Game.Random.Float(180));
                hotdog.LocalRotation = hotdog.LocalRotation.RotateAroundAxis(Vector3.Up, reverse ? 180 : 0);
                hotdog.Reversed = reverse;
                hotdog.Parent = parent;

                slots[index] = slotData;

                break;
            }
        }
    }

    public override void Trigger()
    {
        addHotdog();
    }

    public override void Tick()
    {
        // Do the hotdog logic
    }
}
