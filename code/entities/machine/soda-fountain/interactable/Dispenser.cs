using Sandbox;
using Sandbox.util;
using Sandbox.Utility;

namespace Cinema;

public partial class Dispenser : BaseInteractable
{
    static private float DispenseTime = 3.0f;
    
    private RealTimeUntil TimeUntilFinishedDispensing = 0.0f;
    public bool IsDispensing { get; set; }

    public string AnimationName { get; set; }
    public SodaFountain.SodaType SodaType { get; set; }
    private Particles SodaParticles { get; set; }
    private FillableCup Cup { get; set; }

    public Dispenser() // For the compiler...
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cup"></param>
    public void SetCup(FillableCup cup)
    {
        Cup = cup;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    public override void Trigger(Player player)
    {
        if(!IsDispensing)
        {
            // If an assembled cup is underneath this dispenser,
            // don't dispense until it is picked up
            if (Cup.IsValid() && Cup.Assembled()) return;

            if (Cup.IsValid()) // If empty cup is underneath this dispenser, create soda fill particles
            {
                var particlePath = SodaFountain.GetSodaFillParticlePath(SodaType);

                SodaParticles = Particles.Create(particlePath, Parent);
                SodaParticles.SetEntityAttachment(0, Parent, Attachment);
            }
            else // If no cup is underneath this dispenser, create soda dispense particles
            {
                var particlePath = SodaFountain.GetSodaDispenseParticlePath(SodaType);

                SodaParticles = Particles.Create(particlePath, Parent);
                SodaParticles.SetEntityAttachment(0, Parent, Attachment);
            }

            // Play sound for soda dispensing
            Sound.FromEntity("cup_filling_01", Parent);

            // Reset timer for dispensing 
            TimeUntilFinishedDispensing = DispenseTime;

            IsDispensing = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void Simulate()
    {
        base.Simulate();

        if (TimeUntilFinishedDispensing && IsDispensing)
        {
            // Remove soda dispenser particles
            SodaParticles?.Destroy(true);
            SodaParticles?.Dispose();

            // Assemble cup
            Cup?.Assemble();

            IsDispensing = false;
        }
        
        var leverPos = IsDispensing ? EasingExtensions.EaseArch((float)TimeUntilFinishedDispensing.Fraction).Clamp(0.0f, 1.0f) : 0.0f;
        
        (Parent as AnimatedEntity).SetAnimParameter(AnimationName, leverPos);
    }
    
}
