using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Editor;
using Sandbox;

namespace Cinema;

[Library("cinema_entity_trashdisposal")]
[Title("Trash Bin"), Description("A bin for disposing trash")]
[Model(Model = "models/sbox_props/metal_wheely_bin/metal_wheely_bin.vmdl")]
[HammerEntity]
public partial class TrashDisposal : AnimatedEntity, IUse
{
    [Property, Description("Marks this for janitor use, intended for disposing carrying trash bags")]
    public bool IsJanitorBin { get; set; } = false;

    [Property, Description("How much trash can this bin hold")]
    public int TrashCapacity { get; set; } = 3;

    private int TrashCount;

    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel(PhysicsMotionType.Static);
        TrashCount = 0;
    }

    public bool CanDispose(Carriable active)
    {
        if (active is not WeaponBase trash) return false;

        return TrashCount < TrashCapacity;
    }

    public void UseBin(Player disposer)
    {
        if(disposer.Job.Abilities == Jobs.JobAbilities.PickupGarbage)
        {
            TrashCount = 0;
        } 
        else
        {
            if (!CanDispose(disposer.ActiveChild)) return;

            disposer.Inventory.RemoveWeapon(disposer.ActiveChild, false);

            TrashCount++;
        }
    }

    public bool IsUsable(Entity user)
    {
        if( user is Player player )
        {
            if (IsJanitorBin && player.Job.Abilities != Jobs.JobAbilities.PickupGarbage)
                return false;

            return true;
        }

        return false;
    }

    public bool OnUse(Entity user)
    {
        if (!IsUsable(user)) return false;

        UseBin(user as Player);

        return false;
    }
}
