using Sandbox;
using Sandbox.util;

namespace Cinema;

public class Dispenser : BaseInteractable
{
    public bool CupPlaced { get; set; }
    private string AnimationName { get; set; }
    private bool IsDispensing { get; set; }
    private Particles Soda { get; set; }
    private float LeverPos { get; set; }
    public Dispenser() // For the compiler...
    {
        
    }
    public Dispenser(string animationName) 
    {
        AnimationName = animationName;
    }
    public override void Trigger(Player player)
    {
        IsDispensing = !IsDispensing;

        if (IsDispensing)
        {
            if (CupPlaced)
            {
                Soda = Particles.Create($"particles/soda_fountain/sodafill2_f.vpcf", Parent);
                Soda.SetEntityAttachment(0, Parent, Attachment);
            } 
            else
            {
                Soda = Particles.Create($"particles/soda_fountain/sodafill1_f.vpcf", Parent);
                Soda.SetEntityAttachment(0, Parent, Attachment);
            }
        }
        else
        {
            Soda?.Destroy();
            Soda?.Dispose();
        }
    }

    public override void Simulate()
    {
        base.Simulate();

        if (IsDispensing)
        {
            if(LeverPos < 1.0f)
            {
                LeverPos += 0.05f;
            }
            else
            {
                LeverPos = 1.0f;
            }
        }
        else
        {
            if (LeverPos > 0.0f)
            {
                LeverPos -= 0.05f;
            }
            else
            {
                LeverPos = 0.0f;
            }
        }

        (Parent as AnimatedEntity).SetAnimParameter(AnimationName, LeverPos);
    }
}
