using Sandbox;
using Sandbox.util;

namespace Cinema;

public partial class Dispenser : BaseInteractable
{
    static private float LeverPosIncrement = 0.05f;
    static private float DispenseTime = 3.5f;
    private string AnimationName { get; set; }
    private TimeUntil DispenseTimer = 0.0f;
    private float LeverPos { get; set; }
    public bool IsDispensing { get; set; }
  
    public SodaFountain.SodaType SodaType { get; set; }
    private string SodaFillParticlePath => SodaType switch
    {
        SodaFountain.SodaType.Conk => "particles/soda_fountain/walker/sodafill2_conk_f.vpcf",
        SodaFountain.SodaType.MionPisz => "particles/soda_fountain/walker/sodafill2_mionpisz_f.vpcf",
        SodaFountain.SodaType.Spooge => "particles/soda_fountain/walker/sodafill2_spooge_f.vpcf",

        _ => "particles/soda_fountain/walker/sodafill2_conk_f.vpcf"
    };
    private string SodaDispenseParticlePath => SodaType switch
    {
        SodaFountain.SodaType.Conk => "particles/soda_fountain/walker/sodafill1_nocup_conk_f.vpcf",
        SodaFountain.SodaType.MionPisz => "particles/soda_fountain/walker/sodafill1_nocup_mionpisz_f.vpcf",
        SodaFountain.SodaType.Spooge => "particles/soda_fountain/walker/sodafill1_nocup_spooge_f.vpcf",

        _ => "particles/soda_fountain/walker/sodafill1_nocup_conk_f.vpcf"
    };

    private Particles SodaParticles { get; set; }
    public CupFillable Cup { get; set; }
    
    public Dispenser() // For the compiler...
    {
        
    }
  
    public Dispenser(string animationName, SodaFountain.SodaType type)
    {
        AnimationName = animationName;
        SodaType = type;
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
                SodaParticles = Particles.Create(SodaFillParticlePath, Parent);
                SodaParticles.SetEntityAttachment(0, Parent, Attachment);
            }
            else // If no cup is underneath this dispenser, create soda dispense particles
            {
                SodaParticles = Particles.Create(SodaDispenseParticlePath, Parent);
                SodaParticles.SetEntityAttachment(0, Parent, Attachment);
            }

            // Play sound for soda dispensing
            Sound.FromEntity("cup_filling_01", Parent);
        }
    }
    public override void Simulate()
    {
        base.Simulate();

        if (DispenseTimer && IsDispensing)
        {
            // Remove soda dispenser particles
            SodaParticles?.Destroy(true);
            SodaParticles?.Dispose();

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
