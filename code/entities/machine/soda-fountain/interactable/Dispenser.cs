using Sandbox;
using Sandbox.util;
using System;
namespace Cinema;

public partial class Dispenser : BaseInteractable
{
    static private float LeverPosIncrement = 0.05f;
    static private float DispenseTime = 4.0f;
    private string AnimationName { get; set; }
    public SodaFountain.SodaType SodaType { get; set; }
    private string SodaFillParticlePath { get; set; }
    private string SodaDispenseParticlePath { get; set; }
    private Particles Soda { get; set; }
    public CupFillable Cup { get; set; }
    public bool IsDispensing { get; set; }
    private float LeverPos { get; set; }

    private TimeUntil DispenseTimer = 0.0f;

    public Dispenser() // For the compiler...
    {
        
    }
  
    public Dispenser(string animationName, SodaFountain.SodaType type)
    {
        AnimationName = animationName;
        SodaType = type;

        switch(SodaType)
        {
            case SodaFountain.SodaType.Conk:
                SodaFillParticlePath = "particles/soda_fountain/walker/sodafill2_conk_f.vpcf";
                SodaDispenseParticlePath = "particles/soda_fountain/walker/sodafill1_nocup_conk_f.vpcf";
                break;

            case SodaFountain.SodaType.MionPisz:
                SodaFillParticlePath = "particles/soda_fountain/walker/sodafill2_mionpisz_f.vpcf";
                SodaDispenseParticlePath = "particles/soda_fountain/walker/sodafill1_nocup_mionpisz_f.vpcf";
                break;

            case SodaFountain.SodaType.Spooge:
                SodaFillParticlePath = "particles/soda_fountain/walker/sodafill2_spooge_f.vpcf";
                SodaDispenseParticlePath = "particles/soda_fountain/walker/sodafill1_nocup_spooge_f.vpcf";
                break;
        }
    }

    public override void Trigger(Player player)
    {
        if (!IsDispensing)
        {
            // If an assembled cup is underneath this dispenser,
            // don't dispense until it is picked up
            if (Cup.IsValid() && Cup.Assembled()) return;

            DispenseTimer = DispenseTime;
            IsDispensing = true;

            if (Cup.IsValid()) // If empty cup is underneath this dispenser, create soda fill particles
            {
                Soda = Particles.Create(SodaFillParticlePath, Parent);
                Soda.SetEntityAttachment(0, Parent, Attachment);
            }
            else // If no cup is underneath this dispenser, create soda dispense particles
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
            // Raise Lever
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
            // Lower Lever
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
