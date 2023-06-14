using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_garbagebin"), HammerEntity]
[Model(Archetypes = ModelArchetype.animated_model, Model = "models/sbox_props/metal_wheely_bin/metal_wheely_bin.vmdl")]
[Title("Garbage Bin"), Category("Gameplay"), Icon("delete")]
public partial class GarbageBin : AnimatedEntity
{
    public override void Spawn()
    {
        base.Spawn();
        Model = Cloud.Model("facepunch.metal_wheely_bin");
        SetupPhysicsFromModel(PhysicsMotionType.Static, false);
    }
}
