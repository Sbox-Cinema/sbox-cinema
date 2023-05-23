using Sandbox;
using Sandbox.Component;

namespace Cinema;

public static class EntityExtensions
{

    public static void SetGlow(this Entity entity, bool active, Color color = default)
    {
        if (!entity.IsValid())
            return;

        if (entity is not ModelEntity)
            return;

        var glow = entity.Components.GetOrCreate<Glow>();
        glow.Enabled = active;
        glow.Color = color;
    }

}
