using Sandbox;
using Sandbox.util;

namespace Cinema;

public class Dispenser : BaseInteractable
{
    private string AnimationName { get; set; }
    private int Id { get; set; }

    private bool IsDispensing { get; set; }

    public string Attachment { get; set; }

    public Dispenser() // For the compiler...
    {
        
    }

    public Dispenser(string animationName, string attachment)
    {
        AnimationName = animationName;
        Attachment = attachment;
    }

    public override void Trigger(Player player)
    {
        Log.Info("TRIGGERED");
        Log.Info(Attachment);
    }
}
