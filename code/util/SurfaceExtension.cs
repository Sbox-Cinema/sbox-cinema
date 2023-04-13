namespace Sandbox;

/// <summary>
/// Extensions for Surfaces
/// </summary>
public static partial class SandboxBaseExtensions
{
    public static Particles DoMeleeImpact(this Surface self, TraceResult tr)
    {
        //
        // No effects on resimulate
        //
        if (!Prediction.FirstTime)
            return null;

        //
        // Drop a decal
        //
        var decalPath = Game.Random.FromArray(self.ImpactEffects.BulletDecal);

        var surf = self.GetBaseSurface();
        while (string.IsNullOrWhiteSpace(decalPath) && surf != null)
        {
            decalPath = Game.Random.FromArray(surf.ImpactEffects.BulletDecal);
            surf = surf.GetBaseSurface();
        }

        if (!string.IsNullOrWhiteSpace(decalPath))
        {
            if (ResourceLibrary.TryGet<DecalDefinition>(decalPath, out var decal))
            {
                Decal.Place(decal, tr);
            }
        }

        //
        // Make an impact sound
        //
        var sound = self.Sounds.ImpactHard;

        surf = self.GetBaseSurface();
        while (string.IsNullOrWhiteSpace(sound) && surf != null)
        {
            sound = surf.Sounds.ImpactHard;
            surf = surf.GetBaseSurface();
        }

        if (!string.IsNullOrWhiteSpace(sound))
        {
            Sound.FromWorld(sound, tr.EndPosition);
        }

        //
        // Get us a particle effect
        //

        string particleName = Game.Random.FromArray(self.ImpactEffects.Regular);
        if (string.IsNullOrWhiteSpace(particleName))
            particleName = Game.Random.FromArray(self.ImpactEffects.Regular);

        surf = self.GetBaseSurface();
        while (string.IsNullOrWhiteSpace(particleName) && surf != null)
        {
            particleName = Game.Random.FromArray(surf.ImpactEffects.Regular);
            if (string.IsNullOrWhiteSpace(particleName))
                particleName = Game.Random.FromArray(surf.ImpactEffects.Regular);

            surf = surf.GetBaseSurface();
        }

        if (!string.IsNullOrWhiteSpace(particleName))
        {
            var ps = Particles.Create(particleName, tr.EndPosition);
            ps.SetForward(0, tr.Normal);

            return ps;
        }

        return default;
    }
}
