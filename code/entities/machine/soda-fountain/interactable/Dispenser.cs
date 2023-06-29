using Sandbox;
using Sandbox.util;

namespace Cinema;

public class Dispenser : BaseInteractable
{
    public bool CupPlaced { get; set; }
    private string AnimationName { get; set; }
    private bool IsDispensing { get; set; }
    private Particles Soda { get; set; }
    private string ParticlePath { get; set; }
    private float LeverPos { get; set; }

    static private float LeverPosIncrement = 0.05f;
    public Dispenser() // For the compiler...
    {
        
    }
    public Dispenser(string animationName, string particlePath) 
    {
        AnimationName = animationName;
        ParticlePath = particlePath;
    }
    public override void Trigger(Player player)
    {
        IsDispensing = !IsDispensing;

        if (IsDispensing)
        {
            if (CupPlaced)
            {
                Soda = Particles.Create(ParticlePath, Parent);
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
                LeverPos += LeverPosIncrement;
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
                LeverPos -= LeverPosIncrement;
            }
            else
            {
                LeverPos = 0.0f;
            }
        }

        if(!CupPlaced)
        {
            Soda?.Destroy(true);
            Soda?.Dispose();
        }

        (Parent as AnimatedEntity).SetAnimParameter(AnimationName, LeverPos);
    }
}
