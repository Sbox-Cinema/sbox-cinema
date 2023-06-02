using Sandbox;

namespace Cinema;

public partial class Steam : EntityComponent
{
    static private Vector3 AttachmentOffset = new Vector3(0, 0, 8);
    private Particles Particle {get; set;}

    /// <summary>
    /// Called when this component is activated
    /// When activated, creates steam particle and attaches to entity
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        Particle = Particles.Create("particles/food_steam/hotdogsteam.vpcf");

        Particle.SetEntityAttachment(0, Entity, "steam", AttachmentOffset, ParticleAttachment.Origin);
    }

    /// <summary>
    /// Called when this component is deactivated
    /// When deactivated destroy steam particle
    /// </summary>
    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        Particle.Destroy();
    }
}
