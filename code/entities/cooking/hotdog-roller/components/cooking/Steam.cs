using Sandbox;

namespace Cinema;

public partial class Steam : EntityComponent
{
    private Particles Particle {get; set;}
    protected override void OnActivate()
    {
        base.OnActivate();

        Particle = Particles.Create("particles/food_steam/hotdogsteam.vpcf");

        Particle.SetEntityAttachment(0, Entity, "steam", new Vector3(0, 0, 8), ParticleAttachment.Origin);
    }
    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        Particle.Destroy();
    }
}
