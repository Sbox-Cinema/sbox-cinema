using Sandbox;
using Sandbox.util;
using System;
namespace Cinema;

public partial class Dispenser : BaseInteractable
{
    static private float LeverPosIncrement = 0.05f;
    static private float DispenseTime = 4.0f;
    private string AnimationName { get; set; }
    private string SodaFillParticlePath { get; set; }
    private string SodaDispenseParticlePath { get; set; }
    public CupFillable Cup { get; set; }
    public bool IsDispensing { get; set; }
    private Particles Soda { get; set; }
    private float LeverPos { get; set; }

    private TimeUntil DispenseTimer = 0.0f;
    
    public Dispenser() // For the compiler...
    {
        
    }
    public Dispenser(string animationName, string sodaFillParticlePath, string sodaDispenseParticlePath) 
    {
        AnimationName = animationName;

        SodaFillParticlePath = sodaFillParticlePath;
        SodaDispenseParticlePath = sodaDispenseParticlePath;
    }
    public override void Trigger(Player player)
    {
        if (!IsDispensing)
        {
            if (Cup.IsValid() && Cup.Assembled()) return;

            DispenseTimer = DispenseTime;
            IsDispensing = true;

            if (Cup.IsValid())
            {
                Soda = Particles.Create(SodaFillParticlePath, Parent);
                Soda.SetEntityAttachment(0, Parent, Attachment);
            }
            else
            {
                Soda = Particles.Create(SodaDispenseParticlePath, Parent);
                Soda.SetEntityAttachment(0, Parent, Attachment);
            }
        }
    }
    public override void Simulate()
    {
        base.Simulate();

        if(DispenseTimer && IsDispensing)
        {
            // Remove soda dispenser particles
            Soda?.Destroy(true);
            Soda?.Dispose();

            // Assemble cup
            Cup?.Assemble();

            IsDispensing = false;
        } 
        else if(IsDispensing)
        {
            if (LeverPos < 1.0f)
            {
                LeverPos += LeverPosIncrement;
            }
            else
            {
                LeverPos = 1.0f;
            }
        } 
        else if(!IsDispensing)
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

        (Parent as AnimatedEntity).SetAnimParameter(AnimationName, LeverPos);
   }
}
